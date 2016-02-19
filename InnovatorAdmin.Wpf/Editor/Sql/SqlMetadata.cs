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
      new SqlCommand(@"SELECT SCHEMA_NAME(SCHEMA_ID) sch
                        , name
                        , case when type in ('D') then 'Constraint'
                            when type in ('F', 'PK', 'UQ') then 'Key'
                            when type in ('S', 'IT', 'U') then 'Table'
                            when type in ('IF', 'FN', 'TF') then 'Function'
                            when type in ('P') then 'Procedure'
                            when type in ('V') then 'View'
                            else type end type
                        , case when type in ('IF', 'TF') then 1 
                              when type = 'PK' then 2 else 0 end sub_type
                        , object_id
                        , parent_object_id
                      FROM sys.objects AS SO
                      UNION ALL
                      SELECT 
                          SCHEMA_NAME(o.schema_id) + '.[' + o.name + ']' sch
                        , ind.name
                        , 'Index'
                        , 0 table_valued
                        , ind.index_id object_id
                        , ind.object_id parent_object_id
                      FROM sys.indexes ind
                      inner join sys.[objects] o
                      on o.object_id = ind.object_id
                      WHERE 
                       ind.is_primary_key = 0 
                       AND ind.is_unique = 0 
                       AND ind.is_unique_constraint = 0 
                       AND o.is_ms_shipped = 0", conn)
        .GetListAsync<SqlObject>(async (r) => SqlObject.Create(
          await r.GetFieldStringAsync(0),
          await r.GetFieldStringAsync(1),
          await r.GetFieldStringAsync(2),
          await r.GetFieldIntAsync(4),
          await r.GetFieldIntAsync(5),
          (SqlSubType)await r.GetFieldIntAsync(3)
        )).ContinueWith(l =>
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
            var items = l.Result.Concat(_systemViews);
            _objects = items.ToDictionary(o => o.Schema + "." + o.Name, StringComparer.OrdinalIgnoreCase);
            _schemas = items.Where(o => !string.IsNullOrEmpty(o.Schema)).Select(o => o.Schema).Distinct().ToArray();
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
            && o.SubType == SqlSubType.TableValuedFunction))
        .Select(o => o.Schema + ".[" + o.Name + "]");
    }


    public IEnumerable<string> GetFunctionNames(bool tableValued)
    {
      return _objects.Values
        .Where(o => string.Equals(o.Type, "function", StringComparison.OrdinalIgnoreCase)
            && ((tableValued && o.SubType == SqlSubType.TableValuedFunction)
              || (!tableValued && o.SubType != SqlSubType.TableValuedFunction)))
        .Select(o => o.Schema + ".[" + o.Name + "]");
    }

    public Innovator.Client.IPromise<IEnumerable<IListValue>> GetColumnNames(string tableName)
    {
      return GetColumns(tableName).Convert(l => l.Select(c => (IListValue)new ListValue { Value = c.Name }));
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
        var sql = @"select c.name, t.name + case when c.system_type_id in (56,52,127,48) then '' when c.precision > 0 or c.scale > 0 then isnull('(' + cast(c.PRECISION as nvarchar(10)) + ',' + cast(c.SCALE as nvarchar(10)) + ')', '') else isnull('(' + cast(c.max_length as nvarchar(10)) + ')', '') end data_type, c.*
                    from sys.[columns] c
                    inner join sys.[types] t
                    on c.user_type_id = t.user_type_id
                    where c.object_id = @objectid";

        cmd.Parameters.AddWithValue("@objectid", obj.Id);
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

    private static SqlObject[] _systemViews = new SqlObject[] {
      new SqlObject() { Schema = "sys", Type = "View", Name = "allocation_units" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "objects" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "assembly_modules" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "parameters" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "check_constraints" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "partitions" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "columns" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "periods" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "computed_columns" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "procedures" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "default_constraints" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "sequences" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "events" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "service_queues" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "event_notifications" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "sql_dependencies" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "extended_procedures" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "sql_expression_dependencies" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "foreign_key_columns" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "sql_modules" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "foreign_keys" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "stats" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "function_order_columns" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "stats_columns" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "hash_indexes" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "synonyms" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "identity_columns" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "table_types" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "index_columns" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "tables" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "indexes" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "trigger_event_types" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "key_constraints" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "trigger_events" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "masked_columns" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "triggers" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "numbered_procedure_parameters" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "views" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "numbered_procedures" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "types" },
      new SqlObject() { Schema = "sys", Type = "View", Name = "assembly_types" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "CHECK_CONSTRAINTS" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "REFERENTIAL_CONSTRAINTS" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "COLUMN_DOMAIN_USAGE" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "ROUTINES" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "COLUMN_PRIVILEGES" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "ROUTINE_COLUMNS" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "COLUMNS" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "SCHEMATA" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "CONSTRAINT_COLUMN_USAGE" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "TABLE_CONSTRAINTS" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "CONSTRAINT_TABLE_USAGE" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "TABLE_PRIVILEGES" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "DOMAIN_CONSTRAINTS" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "TABLES" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "DOMAINS" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "VIEW_COLUMN_USAGE" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "KEY_COLUMN_USAGE" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "VIEW_TABLE_USAGE" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "PARAMETERS" },
      new SqlObject() { Schema = "INFORMATION_SCHEMA", Type = "View", Name = "VIEWS" },
      new SqlObject() { Type = "Procedure", Name = "sp_column_privileges" },
      new SqlObject() { Type = "Procedure", Name = "sp_special_columns" },
      new SqlObject() { Type = "Procedure", Name = "sp_columns" },
      new SqlObject() { Type = "Procedure", Name = "sp_sproc_columns" },
      new SqlObject() { Type = "Procedure", Name = "sp_databases" },
      new SqlObject() { Type = "Procedure", Name = "sp_statistics" },
      new SqlObject() { Type = "Procedure", Name = "sp_fkeys" },
      new SqlObject() { Type = "Procedure", Name = "sp_stored_procedures" },
      new SqlObject() { Type = "Procedure", Name = "sp_pkeys" },
      new SqlObject() { Type = "Procedure", Name = "sp_table_privileges" },
      new SqlObject() { Type = "Procedure", Name = "sp_server_info" },
      new SqlObject() { Type = "Procedure", Name = "sp_tables" }
    };

  }
}
