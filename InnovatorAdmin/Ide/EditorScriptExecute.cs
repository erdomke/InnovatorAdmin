using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class EditorScriptExecute : IEditorScript
  {
    private List<IEditorScript> _children;

    public string Action { get { return null; } }
    public bool AutoRun { get { return false; } }
    public IEnumerable<IEditorScript> Children
    {
      get { return _children ?? Enumerable.Empty<IEditorScript>(); }
    }
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


    public Action Execute { get; set; }


    public EditorScriptExecute Add(IEditorScript child)
    {
      if (_children == null)
        _children = new List<IEditorScript>();
      _children.Add(child);
      return this;
    }


    public OutputType PreferredOutput { get { return OutputType.Any; } }
  }
}
