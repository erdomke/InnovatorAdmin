using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class PlainTextEditorHelper : IEditorHelper
  {
    public virtual ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition GetHighlighting()
    {
      return null;
    }

    public virtual IEnumerable<string> GetParameterNames(string query)
    {
      return Enumerable.Empty<string>();
    }

    public virtual IFoldingStrategy FoldingStrategy
    {
      get { return null; }
    }

    public virtual void HandleTextEntered(EditorWinForm control, string insertText)
    {
      // Do nothing
    }

    public virtual string GetCurrentQuery(ITextSource text, int offset)
    {
      return text.Text;
    }

    public virtual Innovator.Client.IPromise<CompletionContext> ShowCompletions(EditorWinForm control)
    {
      return Innovator.Client.Promises.Resolved(new CompletionContext());
    }

    public virtual void Format(System.IO.TextReader reader, System.IO.TextWriter writer)
    {
      // Do nothing
    }

    public virtual void Minify(System.IO.TextReader reader, System.IO.TextWriter writer)
    {
      // Do nothing
    }

    public virtual string LineComment
    {
      get { return string.Empty; }
    }

    public virtual string BlockCommentStart
    {
      get { return string.Empty; }
    }

    public virtual string BlockCommentEnd
    {
      get { return string.Empty; }
    }

    public IEnumerable<IEditorScript> GetScripts(ITextSource text, int offset)
    {
      return Enumerable.Empty<IEditorScript>();
    }

    public virtual IEnumerable<IEditorScript> GetScripts(IEnumerable<System.Data.DataRow> rows, string column)
    {
      return Enumerable.Empty<IEditorScript>();
    }
  }
}
