using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class PlainTextEditorHelper : IEditorHelper
  {
    public ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition GetHighlighting()
    {
      return null;
    }

    public IEnumerable<string> GetParameterNames(string query)
    {
      return Enumerable.Empty<string>();
    }

    public IFoldingStrategy FoldingStrategy
    {
      get { return null; }
    }

    public void HandleTextEntered(EditorWinForm control, string insertText)
    {
      // Do nothing
    }

    public string GetCurrentQuery(string text, int offset)
    {
      return text;
    }

    public Innovator.Client.IPromise<CompletionContext> ShowCompletions(EditorWinForm control)
    {
      return Innovator.Client.Promises.Resolved(new CompletionContext());
    }
  }
}
