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
  public class SqlEditorHelper : IEditorHelper
  {
    private SqlCompletionHelper _sql;

    public SqlEditorHelper(SqlConnection conn)
    {
      _sql = new SqlCompletionHelper(SqlMetadata.Cached(conn));
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


    public Innovator.Client.IPromise<CompletionContext> ShowCompletions(EditorWinForm control)
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
  }
}
