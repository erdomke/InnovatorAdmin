using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Innovator.Client;

namespace InnovatorAdmin.Editor
{
  public class SqlEditorProxy : IEditorProxy
  {
    private Editor.SqlEditorHelper _helper;
    private Editor.PlainTextEditorHelper _outputHelper;
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
      _outputHelper = new Editor.PlainTextEditorHelper();
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

    public Innovator.Client.IPromise<IResultObject> Process(ICommand request, bool async, Action<int, string> progressCallback)
    {
      var intCmd = request as WrappedSqlCommand;
      if (intCmd == null)
        throw new NotSupportedException("Cannot run commands created by a different proxy");

      var cmd = intCmd.Internal;
      _builder.Clear();

      var cts = new CancellationTokenSource();
      cmd.Connection = _conn;
      return cmd.GetResultAsync(cts.Token)
        .ToPromise(cts)
        .Convert(r => {
          r.SetText(_builder.ToString() + Environment.NewLine + r.GetText());
          return (IResultObject)r;
        });
    }

    public static SqlConnection GetConnection(Connections.ConnectionData data, string database = null)
    {
      string connString;
      switch (data.Authentication)
      {
        case Connections.Authentication.Anonymous:
          throw new NotSupportedException("Anonymous authentication is not supported.");
        case Connections.Authentication.Explicit:
          connString = string.Format("server={0};uid={1};pwd={2};database={3};MultipleActiveResultSets=True",
            data.Url, data.UserName, data.Password, database ?? data.Database);
          break;
        case Connections.Authentication.Windows:
          connString = string.Format("server={0};database={1};Trusted_Connection=Yes;MultipleActiveResultSets=True",
            data.Url, database ?? data.Database);
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

    public void Dispose()
    {
      _conn.Dispose();
    }

    private const string ProgrammabilityFolder = "Programmability";

    public IPromise<IEnumerable<IEditorTreeNode>> GetNodes()
    {
      var metadata = SqlMetadata.Cached(_conn);
      return metadata.Objects
        .Convert(RootFolder);
    }

    private IEnumerable<IEditorTreeNode> RootFolder(IEnumerable<SqlObject> objects)
    {
      yield return FolderNode("Tables", objects.Where(o =>
        string.Equals(o.Type, "table", StringComparison.OrdinalIgnoreCase)));
      yield return FolderNode("Views", objects.Where(o =>
        string.Equals(o.Type, "view", StringComparison.OrdinalIgnoreCase)));
      yield return new EditorTreeNode()
      {
        Name = "Programmability",
        Image = Icons.Folder16,
        HasChildren = true,
        Children = new IEditorTreeNode[] {
          FolderNode("Stored Procedures", objects.Where(o =>
            string.Equals(o.Type, "PROCEDURE", StringComparison.OrdinalIgnoreCase))),
          FolderNode("Table-valued Functions", objects.Where(o =>
            string.Equals(o.Type, "FUNCTION", StringComparison.OrdinalIgnoreCase) && o.SubType == SqlSubType.TableValuedFunction)),
          FolderNode("Scalar-valued Functions", objects.Where(o =>
            string.Equals(o.Type, "FUNCTION", StringComparison.OrdinalIgnoreCase) && o.SubType != SqlSubType.TableValuedFunction)),
        }
      };
    }

    private EditorTreeNode FolderNode(string name, IEnumerable<SqlObject> children)
    {
      return new EditorTreeNode()
      {
        Name = name,
        Image = Icons.Folder16,
        HasChildren = true,
        Children = children
          .Where(o => o.Schema != "INFORMATION_SCHEMA" && o.Schema != "sys")
          .Select(GetNode).OrderBy(n => n.Name)
      };
    }

    private IEditorTreeNode GetNode(SqlObject obj)
    {
      var metadata = SqlMetadata.Cached(_conn);

      if (string.Equals(obj.Type, "table", StringComparison.OrdinalIgnoreCase)
        || string.Equals(obj.Type, "view", StringComparison.OrdinalIgnoreCase))
      {
        return new EditorTreeNode()
        {
          Name = obj.Schema + "." + obj.Name,
          Image = Icons.Class16,
          HasChildren = true,
          Children = new IEditorTreeNode[] {
            new EditorTreeNode() {
              Name = "Columns",
              Image = Icons.Folder16,
              HasChildren = true,
              ChildGetter = () => metadata.GetColumns(obj.Name, obj.Schema).Wait()
                .Select(c => new EditorTreeNode()
                {
                  Name = c.Name,
                  Description = c.Type,
                  Image = Icons.Property16,
                }).OrderBy(n => n.Name)
            },
            new EditorTreeNode() {
              Name = "Keys",
              Image = Icons.Folder16,
              HasChildren = metadata.Objects.Wait().Any(o => o.ParentId == obj.Id && o.Type == "Key"),
              ChildGetter = () => metadata.Objects.Wait().Where(o => o.ParentId == obj.Id && o.Type == "Key")
                .Select(c => new EditorTreeNode()
                {
                  Name = c.Name,
                  Image = Icons.Method16,
                }).OrderBy(n => n.Name)
            },
            new EditorTreeNode() {
              Name = "Constraints",
              Image = Icons.Folder16,
              HasChildren = metadata.Objects.Wait().Any(o => o.ParentId == obj.Id && o.Type == "Constraint"),
              ChildGetter = () => metadata.Objects.Wait().Where(o => o.ParentId == obj.Id && o.Type == "Constraint")
                .Select(c => new EditorTreeNode()
                {
                  Name = c.Name,
                  Image = Icons.Method16,
                }).OrderBy(n => n.Name)
            }
            ,
            new EditorTreeNode() {
              Name = "Indexes",
              Image = Icons.Folder16,
              HasChildren = metadata.Objects.Wait().Any(o => o.ParentId == obj.Id && (o.Type == "Index" || o.SubType == SqlSubType.PrimaryKey)),
              ChildGetter = () => metadata.Objects.Wait().Where(o => o.ParentId == obj.Id && (o.Type == "Index" || o.SubType == SqlSubType.PrimaryKey))
                .Select(c => new EditorTreeNode()
                {
                  Name = c.Name,
                  Image = Icons.Method16,
                  Scripts = new IEditorScript[] {
                    new EditorScript() {
                      Name = "Stats",
                      Script = string.Format(@"SELECT
    ind.name AS IndexName
  , OBJECT_NAME(ind.OBJECT_ID) AS TableName
  , indexstats.index_type_desc AS IndexType
  , indexstats.avg_fragmentation_in_percent TotalFragmentation
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, NULL) indexstats
INNER JOIN sys.indexes ind
ON ind.object_id = indexstats.object_id
AND ind.index_id = indexstats.index_id
WHERE ind.index_id = {0} and ind.object_id = {1};", c.Id, c.ParentId)
                    },
                    new EditorScript() {
                      Name = "Rebuild",
                      Script = string.Format("ALTER INDEX [{0}] ON {1} REBUILD PARTITION = ALL WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 95);", c.Name, c.Schema)
                    },
                    new EditorScript() {
                      Name = "Reorganize",
                      Script = string.Format("ALTER INDEX [{0}] ON {1} REORGANIZE WITH ( LOB_COMPACTION = ON );", c.Name, c.Schema)
                    },
                    new EditorScript() {
                      Name = "Disable",
                      Script = string.Format("ALTER INDEX [{0}] ON {1} DISABLE;", c.Name, c.Schema)
                    }
                  }
                }).OrderBy(n => n.Name)
            }
          },
          Scripts = GetScripts(obj)
        };
      }
      else
      {
        return new EditorTreeNode()
        {
          Name = obj.Schema + "." + obj.Name,
          Image = Icons.XmlTag16,
          HasChildren = false,
          Scripts = GetScripts(obj)
        };
      }
    }

    private IEnumerable<EditorScript> GetScripts(SqlObject obj)
    {
      yield return new EditorScript()
      {
        Name = "Create Script",
        ScriptGetter = () =>
        {
          if (string.Equals(obj.Type, "table", StringComparison.OrdinalIgnoreCase))
          {
            var cmd = new SqlCommand(@"DECLARE
      @object_name SYSNAME
    , @object_id INT
    , @SQL NVARCHAR(MAX)
;

SELECT
      @object_name = '[' + OBJECT_SCHEMA_NAME(@id) + '].[' + OBJECT_NAME(@id) + ']'
    , @object_id = @id
;

SELECT SQL = 'CREATE TABLE ' + @object_name + CHAR(13) + '(' + CHAR(13) + STUFF((
    SELECT CHAR(13) + '    , [' + c.name + '] ' +
        CASE WHEN c.is_computed = 1
            THEN 'AS ' + OBJECT_DEFINITION(c.[object_id], c.column_id)
            ELSE
                CASE WHEN c.system_type_id != c.user_type_id
                    THEN '[' + SCHEMA_NAME(tp.[schema_id]) + '].[' + tp.name + ']'
                    ELSE '[' + UPPER(tp.name) + ']'
                END  +
                CASE
                    WHEN tp.name IN ('varchar', 'char', 'varbinary', 'binary')
                        THEN '(' + CASE WHEN c.max_length = -1
                                        THEN 'MAX'
                                        ELSE CAST(c.max_length AS VARCHAR(5))
                                    END + ')'
                    WHEN tp.name IN ('nvarchar', 'nchar')
                        THEN '(' + CASE WHEN c.max_length = -1
                                        THEN 'MAX'
                                        ELSE CAST(c.max_length / 2 AS VARCHAR(5))
                                    END + ')'
                    WHEN tp.name IN ('datetime2', 'time2', 'datetimeoffset')
                        THEN '(' + CAST(c.scale AS VARCHAR(5)) + ')'
                    WHEN tp.name = 'decimal'
                        THEN '(' + CAST(c.[precision] AS VARCHAR(5)) + ',' + CAST(c.scale AS VARCHAR(5)) + ')'
                    ELSE ''
                END +
                CASE WHEN c.collation_name IS NOT NULL AND c.system_type_id = c.user_type_id
                    AND c.collation_name <> 'SQL_Latin1_General_CP1_CI_AS'
                    THEN ' COLLATE ' + c.collation_name
                    ELSE ''
                END +
                CASE WHEN c.is_nullable = 1
                    THEN ' NULL'
                    ELSE ' NOT NULL'
                END +
                CASE WHEN c.default_object_id != 0
                    THEN ' CONSTRAINT [' + OBJECT_NAME(c.default_object_id) + ']' +
                         ' DEFAULT ' + OBJECT_DEFINITION(c.default_object_id)
                    ELSE ''
                END +
                CASE WHEN cc.[object_id] IS NOT NULL
                    THEN ' CONSTRAINT [' + cc.name + '] CHECK ' + cc.[definition]
                    ELSE ''
                END +
                CASE WHEN c.is_identity = 1
                    THEN ' IDENTITY(' + CAST(IDENTITYPROPERTY(c.[object_id], 'SeedValue') AS VARCHAR(5)) + ',' +
                                    CAST(IDENTITYPROPERTY(c.[object_id], 'IncrementValue') AS VARCHAR(5)) + ')'
                    ELSE ''
                END
        END
    FROM sys.columns c WITH(NOLOCK)
    JOIN sys.types tp WITH(NOLOCK) ON c.user_type_id = tp.user_type_id
    LEFT JOIN sys.check_constraints cc WITH(NOLOCK)
         ON c.[object_id] = cc.parent_object_id
        AND cc.parent_column_id = c.column_id
    WHERE c.[object_id] = @object_id
    ORDER BY c.column_id
    FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'), 1, 7, '      ') +
    ISNULL((SELECT '
    , CONSTRAINT [' + i.name + '] PRIMARY KEY ' +
    CASE WHEN i.index_id = 1
        THEN 'CLUSTERED'
        ELSE 'NONCLUSTERED'
    END +' (' + (
    SELECT STUFF(CAST((
        SELECT ', [' + COL_NAME(ic.[object_id], ic.column_id) + ']' +
                CASE WHEN ic.is_descending_key = 1
                    THEN ' DESC'
                    ELSE ''
                END
        FROM sys.index_columns ic WITH(NOLOCK)
        WHERE i.[object_id] = ic.[object_id]
            AND i.index_id = ic.index_id
        FOR XML PATH(N''), TYPE) AS NVARCHAR(MAX)), 1, 2, '')) + ')'
    FROM sys.indexes i WITH(NOLOCK)
    WHERE i.[object_id] = @object_id
        AND i.is_primary_key = 1), '') + CHAR(13) + ');'  ", _conn);
            cmd.Parameters.AddWithValue("id", obj.Id);

            var data = cmd.GetResultAsync();
            data.Wait();
            var tbl = data.Result.GetDataSet().Tables[0];
            if (tbl.Rows.Count < 1)
              return string.Empty;
            return (string)tbl.Rows[0][0];
          }
          else
          {
            var cmd = new SqlCommand(@"select m.definition
from sys.sql_modules m
where m.object_id = @id", _conn);
            cmd.Parameters.AddWithValue("id", obj.Id);

            var data = cmd.GetResultAsync();
            data.Wait();
            var tbl = data.Result.GetDataSet().Tables[0];
            if (tbl.Rows.Count < 1)
              return string.Empty;
            return (string)tbl.Rows[0][0];
          }
        }
      };
      yield return new EditorScript()
      {
        Name = "Drop Script",
        Script = "DROP " + obj.Type.ToUpperInvariant() + " [" + obj.Schema + "].[" + obj.Name + "]"
      };
    }

    public IEditorHelper GetOutputHelper()
    {
      return _outputHelper;
    }
  }
}
