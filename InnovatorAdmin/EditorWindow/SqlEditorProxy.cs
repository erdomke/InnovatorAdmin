using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Innovator.Client;

namespace InnovatorAdmin
{
  public class SqlEditorProxy : IEditorProxy
  {
    private Editor.SqlEditorHelper _helper;
    private SqlConnection _conn;
    private StringBuilder _builder = new StringBuilder();

    public string Action { get; set; }
    public Connections.ConnectionData ConnData { get; private set; }
    public string Name
    {
      get { return this.ConnData.ConnectionName; }
    }

    public SqlEditorProxy(Connections.ConnectionData connData)
    {
      _conn = GetConnection(connData);
      this.ConnData = connData;
      _helper = new Editor.SqlEditorHelper(_conn);
      _conn.InfoMessage += _conn_InfoMessage;
    }

    void _conn_InfoMessage(object sender, SqlInfoMessageEventArgs e)
    {
      _builder.AppendLine(e.Message);
    }

    public IEditorProxy Clone()
    {
      return new SqlEditorProxy(this.ConnData);
    }

    public IEnumerable<string> GetActions()
    {
      return Enumerable.Empty<string>();
    }

    public Editor.IEditorHelper GetHelper()
    {
      return _helper;
    }

    public ICommand NewCommand()
    {
      return new WrappedSqlCommand();
    }

    public Innovator.Client.IPromise<IResultObject> Process(ICommand request, bool async)
    {
      var intCmd = request as WrappedSqlCommand;
      if (intCmd == null)
        throw new NotSupportedException("Cannot run commands created by a different proxy");

      var cmd = intCmd.Internal;
      _builder.Clear();

      var cts = new CancellationTokenSource();
      cmd.Connection = _conn;
      return cmd.GetTableAsync(cts.Token)
        .ToPromise(cts)
        .Convert(t => (IResultObject)new ResultObject(t, _builder.ToString()));
    }

    public static SqlConnection GetConnection(Connections.ConnectionData data)
    {
      string connString;
      switch (data.Authentication)
      {
        case Connections.Authentication.Anonymous:
          throw new NotSupportedException("Anonymous authentication is not supported.");
        case Connections.Authentication.Explicit:
          connString = string.Format("server={0};uid={1};pwd={2};database={3};MultipleActiveResultSets=True",
            data.Url, data.UserName, data.Password, data.Database);
          break;
        case Connections.Authentication.Windows:
          connString = string.Format("server={0};database={1};Trusted_Connection=Yes;MultipleActiveResultSets=True",
            data.Url, data.Database);
          break;
        default:
          throw new NotSupportedException();
      }

      return new SqlConnection(connString);
    }

    private class WrappedSqlCommand : ICommand
    {
      private SqlCommand _cmd;

      public SqlCommand Internal
      {
        get { return _cmd; }
      }

      public WrappedSqlCommand()
      {
        _cmd = new SqlCommand();
      }

      public ICommand WithQuery(string query)
      {
        _cmd.CommandText = query;
        return this;
      }

      public ICommand WithAction(string action)
      {
        return this;
      }

      public ICommand WithParam(string name, object value)
      {
        _cmd.Parameters.AddWithValue(name, value);
        return this;
      }
    }

    private class ResultObject : IResultObject
    {
      private DataTable _table;
      private string _text;

      public ResultObject(DataTable table, string text)
      {
        _table = table;
        _text = text;
      }

      public int ItemCount
      {
        get { return _table.Rows.Count; }
      }

      public string GetText()
      {
        return _text;
      }

      public DataTable GetTable()
      {
        return _table;
      }

      public bool PreferTable
      {
        get { return _table.Rows.Count > 0; }
      }
    }

    public void Dispose()
    {
      _conn.Dispose();
    }
  }
}
