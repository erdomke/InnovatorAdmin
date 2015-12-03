using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using System.Threading;

namespace InnovatorAdmin.Editor
{
  public class SqlEditorHelper : IEditorHelper, ISqlMetadataProvider
  {
    private SqlCompletionHelper _sql;
    private SqlConnection _conn;
    private Dictionary<string, SqlObject> _objects;
    private string[] _schemas;

    public SqlEditorHelper(SqlConnection conn)
    {
      _sql = new SqlCompletionHelper(this);
      _conn = conn;

      new SqlCommand("SELECT * FROM information_schema.tables", conn)
        .GetListAsync<SqlObject>(async (r) => new SqlObject() {
          Schema = await r.GetFieldValueAsync<string>(1),
          Name = await r.GetFieldValueAsync<string>(2),
          Type = await r.GetFieldValueAsync<string>(3)
        }).ContinueWith(l => {
          _objects = l.Result.ToDictionary(o => o.Name, StringComparer.OrdinalIgnoreCase);
          _schemas = l.Result.Select(o => o.Schema).Distinct().ToArray();
        });
    }

    public IEnumerable<string> GetParameterNames(string query)
    {
      return new SqlTokenizer(query)
        .OfType<SqlLiteral>()
        .Where(t => t.Text[0] == '@' && !t.Text.StartsWith("@@"))
        .Select(t => t.Text.Substring(1));
    }

    public IFoldingStrategy FoldingStrategy
    {
      get { return null; }
    }

    public void HandleTextEntered(EditorControl control, string insertText)
    {
      switch (insertText)
      {
        case " ":
        case ",":
        case "(":
        case ".":
          ShowCompletions(control);
          break;
      }
    }

    public string GetCurrentQuery(string text, int offset)
    {
      var parseTree = new SqlTokenizer(text).Parse();
      if (!parseTree.Any())
        return text;

      var currNode = parseTree.NodeByOffset(offset);
      var query = Parents(currNode).Reverse().Skip(1).FirstOrDefault() as SqlGroup;
      if (query == null)
        return text;

      var start = query.StartOffset;
      var endLiteral = LastLiteral(query);
      var end = endLiteral.StartOffset + endLiteral.Text.Length;
      return text.Substring(start, end - start);
    }


    private SqlLiteral LastLiteral(ISqlGroupNode group)
    {
      var last = group.Items.Last();
      var lastGroup = last as ISqlGroupNode;
      while (lastGroup != null)
      {
        last = lastGroup.Items.Last();
        lastGroup = last as ISqlGroupNode;
      }
      return last as SqlLiteral;
    }
    private IEnumerable<SqlNode> Parents(SqlNode curr)
    {
      var parent = curr.Parent;
      while (parent != null)
      {
        yield return parent;
        parent = parent.Parent;
      }
    }

    private static IHighlightingDefinition _highlighter;

    static SqlEditorHelper()
    {
      using (var stream = System.Reflection.Assembly.GetExecutingAssembly()
        .GetManifestResourceStream("InnovatorAdmin.resources.Sql.xshd"))
      {
        using (var reader = new System.Xml.XmlTextReader(stream))
        {
          _highlighter =
              ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
              ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
        }
      }
    }

    public IHighlightingDefinition GetHighlighting()
    {
      return _highlighter;
    }


    public Innovator.Client.IPromise<CompletionContext> ShowCompletions(EditorControl control)
    {
      var length = control.Editor.Document.TextLength;
      var caret = control.Editor.CaretOffset;

      var text = control.Editor.Text;

      return _sql.Completions(text.Substring(0, caret), text, caret, null)
        .UiPromise(control)
        .Convert(data =>
        {
          if (length != control.Editor.Document.TextLength
            || caret != control.Editor.CaretOffset)
          {
            ShowCompletions(control);
            return null;
          }

          if (data.Items.Any())
            control.ShowCompletionWindow(data.Items.Select(c =>
            {
              switch (data.State)
              {
                case CompletionType.SqlGeneral:
                  return new SqlGeneralCompletionData(c);
                case CompletionType.SqlObjectName:
                  return new SqlObjectCompletionData(c, this, control);
              }
              return new BasicCompletionData(c);
            }), data.Overlap);

          return data;
        });
    }

    public IEnumerable<string> GetSchemaNames()
    {
      return _schemas;
    }

    public IEnumerable<string> GetTableNames()
    {
      return _objects.Values.Select(o => o.Schema + ".[" + o.Name + "]");
    }

    public Innovator.Client.IPromise<IEnumerable<string>> GetColumnNames(string tableName)
    {
      SqlObject obj;
      if (!_objects.TryGetValue(tableName, out obj))
        return Promises.Resolved(Enumerable.Empty<string>());

      if (obj.Columns == null)
      {
        var cmd = new SqlCommand(
          @"SELECT column_name
              , data_type + isnull('(' + cast(character_maximum_length as nvarchar(10)) + ')', '') + case DATA_TYPE when 'int' then '' else isnull('(' + cast(NUMERIC_PRECISION as nvarchar(10)) + ',' + cast(NUMERIC_SCALE as nvarchar(10)) + ')', '') end data_type
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_NAME = @table", _conn);
        cmd.Parameters.AddWithValue("table", tableName);

        var cts = new CancellationTokenSource();
        obj.Columns = cmd.GetListAsync<SqlColumn>(async (r) => new SqlColumn()
          {
            Name = await r.GetFieldValueAsync<string>(0),
            Type = await r.GetFieldValueAsync<string>(1)
          }, cts.Token)
          .ToPromise(cts)
          .Convert(c => (IEnumerable<SqlColumn>)c);
      }

      return obj.Columns.Convert(l => l.Select(c => c.Name));
    }
  }
}
