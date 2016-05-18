using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class SqlBatchWriter : IDisposable
  {
    private IConnection _conn;
    private StringBuilder _builder;
    private ParameterSubstitution _subs;
    private int _commands = 0;
    private string _lastQuery;
    private IPromise<Stream> _lastResult = null;

    public int Threshold { get; set; }

    public SqlBatchWriter(IConnection conn) : this(conn, 96) { }
    public SqlBatchWriter(IConnection conn, int capacity)
    {
      _builder = new StringBuilder(capacity).Append("<sql>");
      _conn = conn;
      _subs = new ParameterSubstitution()
      {
        Mode = ParameterSubstitutionMode.Sql
      };
      this.Threshold = 3000;
    }

    public SqlBatchWriter Command()
    {
      _builder.AppendLine();
      ProcessCommand(false);
      return this;
    }
    public SqlBatchWriter Command(string value)
    {
      _builder.AppendEscapedXml(value).AppendLine();
      ProcessCommand(false);
      return this;
    }
    public SqlBatchWriter Command(string format, params object[] args)
    {
      _subs.AddIndexedParameters(args);
      _builder.AppendEscapedXml(_subs.Substitute(format, _conn.AmlContext.LocalizationContext)).AppendLine();
      ProcessCommand(false);
      _subs.ClearParameters();
      return this;
    }
    public SqlBatchWriter Part(string value)
    {
      _builder.AppendEscapedXml(value);
      return this;
    }
    public SqlBatchWriter Part(string format, params object[] args)
    {
      _subs.AddIndexedParameters(args);
      _builder.AppendEscapedXml(_subs.Substitute(format, _conn.AmlContext.LocalizationContext)).AppendLine();
      _subs.ClearParameters();
      return this;
    }

    private void ProcessCommand(bool force)
    {
      _commands++;
      if ((force || _commands > this.Threshold) && _builder.Length > 5)
      {
        // Execute the query
        _builder.Append("</sql>");
        WaitLastResult();
        _lastQuery = _builder.ToString();

        // Run either synchronously or asynchronously based on what's currently supported
        var asyncConn = _conn as IAsyncConnection;
        if (asyncConn == null)
        {
          _conn.Apply(new Command(_lastQuery).WithAction(CommandAction.ApplySQL)).AssertNoError();
        }
        else
        {
          _lastResult = asyncConn.Process(new Command(_lastQuery).WithAction(CommandAction.ApplySQL), true);
        }

        // Reset the state
        _builder.Length = 0;
        _builder.Append("<sql>");
        _commands = 0;
      }
    }

    private void WaitLastResult()
    {
      if (_lastResult != null)
      {
        _conn.AmlContext.FromXml(_lastResult.Wait(), _lastQuery, _conn).AssertNoError();
        _lastResult = null;
      }
    }

    public override string ToString()
    {
      return _builder.ToString() + "</sql>";
    }

    public void Flush()
    {
      ProcessCommand(true);
      WaitLastResult();
    }

    public void Dispose()
    {
      this.Flush();
    }
  }
}
