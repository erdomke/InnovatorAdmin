using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace InnovatorAdmin.Editor
{
  public interface IEditorHelper
  {
    string LineComment { get; }
    string BlockCommentStart { get; }
    string BlockCommentEnd { get; }

    void Format(TextReader reader, TextWriter writer);
    void Minify(TextReader reader, TextWriter writer);
    IHighlightingDefinition GetHighlighting();
    IEnumerable<string> GetParameterNames(string query);
    IFoldingStrategy FoldingStrategy { get; }
    void HandleTextEntered(EditorWinForm control, string insertText);
    string GetCurrentQuery(ITextSource text, int offset);
    IPromise<CompletionContext> ShowCompletions(EditorWinForm control);
    IEnumerable<IEditorScript> GetScripts(ITextSource text, int offset);
    IEnumerable<IEditorScript> GetScripts(IEnumerable<DataRow> rows, string column);
  }
}
