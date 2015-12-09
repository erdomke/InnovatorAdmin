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
      textArea.Document.Replace(completionSegment, this.Text);

      if (!this.MultiValue)
      {
        var offset = completionSegment.Offset + this.Text.Length;
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
