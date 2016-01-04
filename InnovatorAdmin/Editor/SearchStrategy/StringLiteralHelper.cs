using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Media;

namespace InnovatorAdmin.Editor
{
  public class StringLiteralHelper : PlainTextEditorHelper
  {
    private InsightWindow _currentInsight;

    public override ICSharpCode.AvalonEdit.Highlighting.IHighlightingDefinition GetHighlighting()
    {
      return _highlighter;
    }

    public override void HandleTextEntered(EditorWinForm control, string insertText)
    {
      switch (insertText)
      {
        case "\\":
          ShowCompletions(control);
          break;
      }
    }

    private static string[][] _completionInfo = new string[][] {
      new string[] { "\\", "Backslash"},
      new string[] { "\'", "Single quote"},
      new string[] { "\"", "Double quote"},
      new string[] { "0", "Null Character (\\u0000)"},
      new string[] { "b", "Backspace (in a character class) (\\u0008)"},
      new string[] { "t", "Tab (\\u0009)"},
      new string[] { "r", "Carriage Return (\\u000d)"},
      new string[] { "v", "Vertical Tab (\\u000b)"},
      new string[] { "f", "Form Feed (\\u000c)"},
      new string[] { "n", "New Line (\\u000a)"},
      new string[] { "x", "Hexadecimal code (e.g. '\\x20')"},
      new string[] { "u", "Unicode hexadecimal code (e.g. '\\u0020')"},
      new string[] { "U", "Unicode hexadecimal code (e.g. '\\U00000020')"},
    };

    public override Innovator.Client.IPromise<CompletionContext> ShowCompletions(EditorWinForm control)
    {
      var length = control.Editor.Document.TextLength;
      var caret = control.Editor.CaretOffset;
      var data = new CompletionContext();

      if (caret > 0)
      {
        string[][] completions = null;
        if (control.Editor.Document.GetCharAt(caret - 1) == '\\')
          completions = _completionInfo;

        if (completions != null)
        {
          data.Items = completions
            .OrderBy(i => i[0].ToLowerInvariant())
            .ThenBy(i => i[1])
            .Select(i => new BasicCompletionData() {
              Content = GetSpan(new Run(i[0] + " "), new Run(i[1])
              {
                Foreground = Brushes.Gray
              }),
              Text = i[0],
              Action = () => i[0]
            });
        }
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

      return Promises.Resolved(data);
    }

    private static Span GetSpan(params Run[] runs)
    {
      var result = new Span();
      result.Inlines.AddRange(runs);
      return result;
    }

    internal static IHighlightingDefinition _highlighter;

    static StringLiteralHelper()
    {
      using (var stream = System.Reflection.Assembly.GetExecutingAssembly()
        .GetManifestResourceStream("InnovatorAdmin.resources.StringLiteral.xshd"))
      {
        using (var reader = new System.Xml.XmlTextReader(stream))
        {
          _highlighter =
              ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
              ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
        }
      }
    }

  }
}
