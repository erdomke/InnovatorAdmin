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
  public partial class AmlEditorHelper : AmlSimpleEditorHelper
  {
    private bool _isInitialized = false;

    public string SoapAction { get; set; }

    public AmlEditorHelper() : base()
    {
      this.SoapAction = "ApplyAML";
      _sql = new SqlCompletionHelper(this);
    }

    public override void HandleTextEntered(EditorWinForm control, string insertText)
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
          case "!":
          case "|":
            _sql.CurrentTextArea = control.Editor.TextArea;
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
            var endTag = this.LastOpenTag(control.Document.CreateSnapshot(0, control.Editor.CaretOffset));
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

    public override IPromise<CompletionContext> ShowCompletions(EditorWinForm control)
    {
      var length = control.Editor.Document.TextLength;
      var caret = control.Editor.CaretOffset;

      if (control.Editor.CaretOffset >= 2 && control.Editor.Document.GetText(control.Editor.CaretOffset - 2, 2) == "<!")
      {
        var context = new CompletionContext()
        {
          Items = new ICompletionData[] {
            new BasicCompletionData() {
              Text = "--",
              Content = "-- (Comment)",
              Action = () => "---->",
              CaretOffset = -3
            },
            new BasicCompletionData() {
              Text = "[CDATA[",
              Action = () => "[CDATA[]]>",
              CaretOffset = -3
            }
          }
        };

        control.ShowCompletionWindow(context.Items, context.Overlap);

        return Promises.Resolved(context);
      }
      else
      {
        return this.GetCompletions(control.Document, control.Editor.CaretOffset, this.SoapAction)
          .ToPromise()
          .UiPromise(control)
          .Convert(data =>
          {
            if (length != control.Editor.Document.TextLength
              || caret != control.Editor.CaretOffset)
            {
              ShowCompletions(control);
              return null;
            }

            if (data?.Items.Any() == true)
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

    public override IEnumerable<string> GetParameterNames(string query)
    {
      var paramNames = new List<string>();
      var subs = new ParameterSubstitution()
      {
        ParameterAccessListener = (n) => paramNames.Add(n)
      };
      subs.Substitute(query, _conn.AmlContext.LocalizationContext);

      return paramNames.Distinct().OrderBy(n => n);
    }



  }
}
