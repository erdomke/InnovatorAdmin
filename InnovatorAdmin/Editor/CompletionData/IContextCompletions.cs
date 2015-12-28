using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public interface IContextCompletions : ICompletionData
  {
    void SetContext(IEditorHelper parent, EditorWinForm control);
  }
}
