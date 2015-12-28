using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class AmlSimpleEditorHelper : IEditorHelper
  {
    private AmlFoldingStrategy _foldingStrategy = new AmlFoldingStrategy();

    public ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition GetHighlighting()
    {
      return AmlEditorHelper._highlighter;
    }

    public IEnumerable<string> GetParameterNames(string query)
    {
      return Enumerable.Empty<string>();
    }

    public IFoldingStrategy FoldingStrategy
    {
      get { return _foldingStrategy; }
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
      return Promises.Resolved(new CompletionContext());
    }
  }
}
