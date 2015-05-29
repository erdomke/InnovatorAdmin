using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  public interface IEditorHelper
  {
    void HandleTextEntered(EditorControl control, string insertText);
    string GetCurrentQuery(string text, int offset);
  }
}
