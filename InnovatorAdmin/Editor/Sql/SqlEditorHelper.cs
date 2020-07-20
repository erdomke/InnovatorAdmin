using ICSharpCode.AvalonEdit.Highlighting;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using System.Threading;
using ICSharpCode.AvalonEdit.Document;

namespace InnovatorAdmin.Editor
{
  public class SqlEditorHelper : IEditorHelper
  {
    private SqlCompletionHelper _sql;

    public virtual string BlockCommentEnd { get { return "*/"; } }
    public virtual string BlockCommentStart { get { return "/*"; } }
    public virtual string LineComment { get { return "--"; } }

    public SqlEditorHelper(SqlConnection conn)
    {
      _sql = new SqlCompletionHelper(SqlMetadata.Cached(conn));
    }

    public IEnumerable<string> GetParameterNames(string query)
    {
      var tokens = new SqlTokenizer(query).ToArray();
      var parsed = SqlTokenizer.Parse(tokens);
      var declares = new SqlDeclares(parsed);
      var declared = new HashSet<string>(declares.Names ?? Enumerable.Empty<string>());

      return new SqlTokenizer(query)
        .OfType<SqlLiteral>()
        .Where(t => t.Text[0] == '@' && !t.Text.StartsWith("@@") && !declared.Contains(t.Text))
        .Select(t => t.Text.Substring(1))
        .Distinct();
    }



    public IFoldingStrategy FoldingStrategy
    {
      get { return null; }
    }

    public void HandleTextEntered(EditorWinForm control, string insertText)
    {
      switch (insertText)
      {
        case " ":
        case ",":
        case "(":
        case ".":
          _sql.CurrentTextArea = control.Editor.TextArea;
          ShowCompletions(control);
          break;
      }
    }

    public string GetCurrentQuery(ITextSource source, int offset)
    {
      var text = source.Text;
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

    public IEnumerable<IEditorScript> GetScripts(ITextSource text, int offset, bool readOnly)
    {
      return Enumerable.Empty<IEditorScript>();
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


    public Innovator.Client.IPromise<CompletionContext> ShowCompletions(EditorWinForm control)
    {
      var length = control.Editor.Document.TextLength;
      var caret = control.Editor.CaretOffset;

      var text = control.Editor.Text;

      return _sql.Completions(text.Substring(0, caret), control.Document, caret, null)
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
          {
            var items = data.Items.ToArray();
            var contextItems = items.OfType<IContextCompletions>();
            foreach (var contextItem in contextItems)
            {
              contextItem.SetContext(this, control);
            }

            control.ShowCompletionWindow(items, data.Overlap);
          }

          return data;
        });
    }

    public void Format(System.IO.TextReader reader, System.IO.TextWriter writer)
    {
      // Do nothing, for now
    }

    public void Minify(System.IO.TextReader reader, System.IO.TextWriter writer)
    {
      // Do nothing, for now
    }

    public virtual IEnumerable<IEditorScript> GetScripts(IEnumerable<System.Data.DataRow> rows, string column)
    {
      return Enumerable.Empty<IEditorScript>();
    }
  }
}
