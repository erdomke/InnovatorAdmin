using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class AttributeValueCompletionData : BasicCompletionData
  {
    public bool MultiValue { get; set; }

    public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
      var insertion = Action == null ? this.Text : Action.Invoke();
      textArea.Document.Replace(completionSegment, insertion);
      base.Complete(textArea, completionSegment, insertionRequestEventArgs);

      if (!this.MultiValue)
      {
        var offset = completionSegment.Offset + insertion.Length;
        var doc = textArea.Document;
        var quote = doc.GetCharAt(completionSegment.Offset - 1);
        if (doc.TextLength > offset && doc.GetCharAt(offset) == quote
          && (quote == '\'' || quote == '"'))
        {
          textArea.Caret.Offset += 1;
        }
      }
    }
  }
}
