using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class SqlMetadata : ISqlMetadataProvider
  {
    private SqlConnection _conn;
    private Dictionary<string, SqlObject> _objects;
    private string[] _schemas;
    private Promise<IEnumerable<SqlObject>> _objectPromise;

    private SqlMetadata(SqlConnection conn)
    {
      _conn = conn;

      _objectPromise = new Promise<IEnumerable<SqlObject>>();
      new SqlCommand(@"SELECT *, null data_type, null defin
                      FROM information_schema.tables
                      where table_type <> 'View'
                      union all
                      SELECT table_catalog, table_schema, table_name, 'View', null, view_definition
                      FROM information_schema.views
                      union all
                      select routine_catalog, routine_schema, routine_name, routine_type, data_type, routine_definition
                      from information_schema.routines", conn)
        .GetListAsync<SqlObject>(async (r) => new SqlObject()
        {
          Schema = await r.GetFieldStringAsync(1),
          Name = await r.GetFieldStringAsync(2),
          Type = GetObjectType(await r.GetFieldStringAsync(3)),
          IsTableValued = string.Equals(await r.GetFieldStringAsync(4), "table", StringComparison.OrdinalIgnoreCase),
          Definition = await r.GetFieldStringAsync(5)
        }).ContinueWith(l =>
        {
          if (l.IsFaulted)
          {
            _objectPromise.Reject(l.Exception);
          }
          else if (l.IsCanceled)
          {
            _objectPromise.Cancel();
          }
          else
          {
            _objects = l.Result.ToDictionary(o => o.Schema + "." + o.Name, StringComparer.OrdinalIgnoreCase);
            _schemas = l.Result.Select(o => o.Schema).Distinct().ToArray();
            _objectPromise.Resolve(_objects.Values);
          }
        });
    }




    private string GetObjectType(string type)
    {
      switch (type.ToLowerInvariant())
      {
        case "base table":
        case "table":
          return "Table";
        case "view":
          return "View";
        default:
          return type;
      }
    }

    public IPromise<IEnumerable<SqlObject>> Objects { get { return _objectPromise; } }

    public IEnumerable<string> GetSchemaNames()
    {
      return _schemas;
    }

    public IEnumerable<string> GetTableNames()
    {
      return _objects.Values
        .Where(o => string.Equals(o.Type, "table", StringComparison.OrdinalIgnoreCase)
          || string.Equals(o.Type, "view", StringComparison.OrdinalIgnoreCase)
          || (string.Equals(o.Type, "function", StringComparison.OrdinalIgnoreCase)
            && o.IsTableValued))
        .Select(o => o.Schema + ".[" + o.Name + "]");
    }

    public Innovator.Client.IPromise<IEnumerable<ListValue>> GetColumnNames(string tableName)
    {
      return GetColumns(tableName).Convert(l => l.Select(c => new ListValue { Value = c.Name }));
    }
    public Innovator.Client.IPromise<IEnumerable<SqlColumn>> GetColumns(string tableName, string schema = null)
    {
      SqlObject obj;
      var searchKey = tableName;
      if (!string.IsNullOrWhiteSpace(schema))
        searchKey = schema + "." + tableName;
      if (!_objects.TryGetValue(searchKey, out obj))
        return Promises.Resolved(Enumerable.Empty<SqlColumn>());

      if (obj.Columns == null)
      {
        if (string.IsNullOrWhiteSpace(schema) && tableName.IndexOf('.') > 0)
        {
          var parts = tableName.Split('.');
          tableName = parts.Last();
          schema = parts.First();
        }

        var cmd = new SqlCommand();
        cmd.Connection = _conn;
        var sql = @"SELECT column_name
              , data_type + isnull('(' + cast(character_maximum_length as nvarchar(10)) + ')', '') + case DATA_TYPE when 'int' then '' else isnull('(' + cast(NUMERIC_PRECISION as nvarchar(10)) + ',' + cast(NUMERIC_SCALE as nvarchar(10)) + ')', '') end data_type
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = @table";

        if (!string.IsNullOrWhiteSpace(schema))
        {
          sql += " AND TABLE_SCHEMA = @schema";
          cmd.Parameters.AddWithValue("schema", schema);
        }

        cmd.Parameters.AddWithValue("table", tableName);
        cmd.CommandText = sql;

        var cts = new CancellationTokenSource();
        obj.Columns = cmd.GetListAsync<SqlColumn>(async (r) => new SqlColumn()
        {
          Name = await r.GetFieldValueAsync<string>(0),
          Type = await r.GetFieldValueAsync<string>(1)
        }, cts.Token)
          .ToPromise(cts)
          .Convert(c => (IEnumerable<SqlColumn>)c.OrderBy(l => l.Name));
      }

      return obj.Columns;
    }


    private static Dictionary<string, SqlMetadata> _cache = new Dictionary<string, SqlMetadata>();
    public static SqlMetadata Cached(SqlConnection conn)
    {
      SqlMetadata result;
      if (!_cache.TryGetValue(conn.ConnectionString, out result))
      {
        result = new SqlMetadata(conn);
        _cache[conn.ConnectionString] = result;
      }
      return result;
    }
  }
}
