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
    private EditorControl _control;
    private string _fullName;

    public override string Text
    {
      get { return base.Text; }
      set
      {
        base.Text = value;
        if (_fullName == null)
        {
          var idx = value.IndexOf('.');
          if (idx > 0 && idx < value.Length - 1)
          {
            _fullName = this.Text;
            this.Text = value.Substring(idx + 1).TrimStart('[').TrimEnd(']');
          }
          else
          {
            _fullName = this.Text;
          }
        }
      }
    }

    public void SetContext(IEditorHelper parent, EditorControl control)
    {
      _parent = parent;
      _control = control;
    }

    public override void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
    {
      var idx = _fullName.IndexOf('.');
      if (idx > 0 && idx < _fullName.Length - 1)
      {
        if (textArea.Document.Text.Substring(completionSegment.Offset - idx + 1, idx + 1).Equals(_fullName.Substring(0, idx + 1), StringComparison.OrdinalIgnoreCase))
        {
          textArea.Document.Replace(completionSegment, "[" + this.Text + "]");
        }
        else
        {
          textArea.Document.Replace(completionSegment, _fullName);
        }
      }
      else
      {
        textArea.Document.Replace(completionSegment, _fullName + ".");
        _parent.ShowCompletions(_control);
      }
    }
  }
}
