using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class AttributeCompletionData : BasicCompletionData, IContextCompletions
  {
    private IEditorHelper _parent;
    private EditorControl _control;


    public void SetContext(IEditorHelper parent, EditorControl control)
    {
      _parent = parent;
      _control = control;
    }

    public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
      textArea.Document.Replace(completionSegment, this.Text + "=''");
      textArea.Caret.Offset -= 1;
      _parent.ShowCompletions(_control);
    }
  }
}
