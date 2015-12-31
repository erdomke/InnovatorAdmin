using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class SqlObjectCompletionData : BasicCompletionData, IContextCompletions
  {
    private IEditorHelper _parent;
    private EditorWinForm _control;

    public string InsertionText { get; set; }

    public void SetContext(IEditorHelper parent, EditorWinForm control)
    {
      _parent = parent;
      _control = control;
    }

    public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
      var insertion = this.InsertionText ?? this.Text;
      textArea.Document.Replace(completionSegment, insertion);
      if (insertion.EndsWith("."))
        _parent.ShowCompletions(_control);
    }
  }
}
