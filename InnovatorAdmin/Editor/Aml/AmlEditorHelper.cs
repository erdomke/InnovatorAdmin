using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InnovatorAdmin.Editor
{
  public class AmlEditorHelper : CompletionHelper, IEditorHelper
  {
    private bool _isInitialized = false;
    private AmlFoldingStrategy _foldingStrategy = new AmlFoldingStrategy();

    public IFoldingStrategy FoldingStrategy
    {
      get { return _foldingStrategy; }
    }
    public string SoapAction { get; set; }

    public AmlEditorHelper()
    {
      this.SoapAction = "ApplyAML";
      _foldingStrategy.ShowAttributesWhenFolded = true;
    }

    public override void InitializeConnection(IAsyncConnection conn)
    {
      _isInitialized = true;
      base.InitializeConnection(conn);
    }

    public void HandleTextEntered(EditorWinForm control, string insertText)
    {
      if (_isInitialized)
      {
        switch (insertText)
        {
          case "'":
          case "\"":
          case " ":
          case "<":
          case ",":
          case "(":
          case ".":
            ShowCompletions(control)
              .Done(data =>
              {
                if (data != null && data.IsXmlTag && !data.Items.Any()
                  && control.Editor.CaretOffset < control.Editor.Document.TextLength)
                {
                  var doc = control.Editor.TextArea.Document;
                  var quote = doc.GetCharAt(doc.LastIndexOf('=', 0, control.Editor.CaretOffset) + 1);
                  if (insertText[0] == quote && quote == doc.GetCharAt(control.Editor.CaretOffset))
                  {
                    doc.Remove(control.Editor.CaretOffset, 1);
                  }
                }
              });
            break;
          case ">":
            var endTag = this.LastOpenTag(control.Editor.Text.Substring(0, control.Editor.CaretOffset));
            if (!string.IsNullOrEmpty(endTag))
            {
              var insert = "</" + endTag + ">";
              if (!control.Editor.Text.Substring(control.Editor.CaretOffset).StartsWith(insert))
              {
                control.Editor.Document.Insert(control.Editor.CaretOffset, insert, AnchorMovementType.BeforeInsertion);
                control.HideCompletionWindow();
              }
            }
            ShowCompletions(control);
            break;
        }
      }
    }

    public IPromise<CompletionContext> ShowCompletions(EditorWinForm control)
    {
      var length = control.Editor.Document.TextLength;
      var caret = control.Editor.CaretOffset;

      return this.GetCompletions(control.Editor.Text, control.Editor.CaretOffset, this.SoapAction)
        .UiPromise(control)
        .Convert(data => {
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

    public string GetCurrentQuery(string text, int offset)
    {
      return this.GetQuery(text, offset);
    }

    public IEnumerable<string> GetParameterNames(string query)
    {
      var paramNames = new List<string>();
      var subs = new ParameterSubstitution()
      {
        ParameterAccessListener = (n) => paramNames.Add(n)
      };
      subs.Substitute(query, _conn.AmlContext.LocalizationContext);

      return paramNames.Distinct().OrderBy(n => n);
    }

    internal static IHighlightingDefinition _highlighter;

    static AmlEditorHelper()
    {
      using (var stream = System.Reflection.Assembly.GetExecutingAssembly()
        .GetManifestResourceStream("InnovatorAdmin.resources.Aml.xshd"))
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
  }
}
