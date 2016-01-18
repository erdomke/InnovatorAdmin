using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  public class DbCommand : Command, IDbCommand
  {
    private DbConnection _conn;
    private DbParams _params = new DbParams();

    public DbCommand(DbConnection conn)
    {
      _conn = conn;
      this.CommandTimeout = DefaultHttpService.DefaultTimeout;
    }

    void IDbCommand.Cancel()
    {
      throw new NotSupportedException();
    }

    public string CommandText
    {
      get { return this.Aml; }
      set { this.Aml = value; }
    }

    public int CommandTimeout { get; set; }

    public CommandType CommandType
    {
      get { return CommandType.Text; }
      set { throw new NotSupportedException(); }
    }

    public IDbConnection Connection
    {
      get { return _conn; }
      set { throw new NotSupportedException(); }
    }

    public IDbDataParameter CreateParameter()
    {
      var result = new DbParameter();
      _params.Add(result);
      return result;
    }
    public DbParameter CreateParameter(string name, object value)
    {
      var result = new DbParameter() { ParameterName = name, Value = value };
      _params.Add(result);
      return result;
    }

    private int ProcessParams()
    {
      if (_params.Any())
      {
        var paramSub = new ParameterSubstitution();
        foreach (var param in _params)
        {
          paramSub.AddParameter(param.ParameterName, param.Value);
        }
        this.Aml = paramSub.Substitute(this.Aml, _conn.CoreConnection.AmlContext.LocalizationContext);
        return paramSub.ItemCount;
      }
      return 1;
    }

    public int ExecuteNonQuery()
    {
      this.Action = ProcessParams() > 1 ? CommandAction.ApplyAML : CommandAction.ApplyItem;
      var result = _conn.CoreConnection.Apply(this);
      result.AssertNoError();
      return result.Items().Count();
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
      this.Action = ProcessParams() > 1 ? CommandAction.ApplyAML : CommandAction.ApplyItem;
      var result = _conn.CoreConnection.Apply(this);
      return new DbReader(_conn, result.Items());
    }

    public IDataReader ExecuteReader()
    {
      return ExecuteReader(CommandBehavior.Default);
    }

    public object ExecuteScalar()
    {
      this.Action = ProcessParams() > 1 ? CommandAction.ApplyAML : CommandAction.ApplyItem;
      var result = _conn.CoreConnection.Apply(this);
      return result.AssertNoError().Value;
    }

    public IDataParameterCollection Parameters
    {
      get { return _params; }
    }

    public void Prepare()
    {
      // Do nothing
    }

    public IDbTransaction Transaction { get; set; }

    public UpdateRowSource UpdatedRowSource { get; set; }

    public void Dispose()
    {
      // Do nothing
    }
  }
}
