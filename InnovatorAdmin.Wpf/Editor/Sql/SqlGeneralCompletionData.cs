using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class SqlGeneralCompletionData : BasicCompletionData, IContextCompletions
  {
    private IEditorHelper _parent;
    private CodeEditor _control;

    public void SetContext(IEditorHelper parent, CodeEditor control)
    {
      _parent = parent;
      _control = control;
    }

    public override void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ICSharpCode.AvalonEdit.Document.ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
      var insertion = Action == null ? this.Text : Action.Invoke();
      textArea.Document.Replace(completionSegment, insertion);
      if (insertion.EndsWith("("))
        _parent.ShowCompletions(_control);
    }
  }
}
