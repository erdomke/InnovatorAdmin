using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class EditorScriptExecute : IEditorScript
  {
    public string Action { get { return null; } }

    public string Name { get; set; }

    public string Script
    {
      get
      {
        if (Execute != null)
          Execute();
        return null;
      }
    }

    public bool AutoRun { get { return false; } }

    public Action Execute { get; set; }
  }
}
