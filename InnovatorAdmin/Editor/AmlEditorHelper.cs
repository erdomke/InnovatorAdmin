using Aras.AutoComplete;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  public class AmlEditorHelper : CompletionHelper, IEditorHelper
  {
    private bool _isInitialized = false;
    
    public string SoapAction { get; set; }

    public AmlEditorHelper()
    {
      this.SoapAction = "ApplyAML";
    }

    public override void InitializeConnection(Func<string, XmlNode, XmlNode> applyAction)
    {
      _isInitialized = true;
      base.InitializeConnection(applyAction);
    }

    public void HandleTextEntered(EditorControl control, string insertText)
    {
      if (_isInitialized)
      {
        switch (insertText)
        {
          case "\"":
          case " ":
          case "<":
          case ",":
          case "(":
            int overlap;
            var completionItems = this.GetCompletions(control.Editor.Text.Substring(0, control.Editor.CaretOffset), this.SoapAction, out overlap).Select(c => new BasicCompletionData(c));
            control.ShowCompletionWindow(completionItems, overlap);
            break;
          case ">":
            var endTag = this.LastOpenTag(control.Editor.Text.Substring(0, control.Editor.CaretOffset));
            if (!string.IsNullOrEmpty(endTag))
            {
              var insert = "</" + endTag + ">";
              if (!control.Editor.Text.Substring(control.Editor.CaretOffset).StartsWith(insert))
              {
                control.Editor.Document.Insert(control.Editor.CaretOffset, insert, AnchorMovementType.BeforeInsertion);
              }
            }
            break;
        }
      }
    }

    public string GetCurrentQuery(string text, int offset)
    {
      return this.GetQuery(text, offset);
    }
  }
}