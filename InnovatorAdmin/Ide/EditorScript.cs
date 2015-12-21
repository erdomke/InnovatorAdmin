using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class EditorScript : IEditorScript
  {
    private string _script;
    private bool _scriptLoaded;

    public string Name { get; set; }
    public string Action { get; set; }
    public string Script 
    {
      get 
      {
        if (!_scriptLoaded)
        {
          _scriptLoaded = true;
          if (this.ScriptGetter != null)
          {
            Utils.CallWithTimeout(5000, () => _script = this.ScriptGetter.Invoke());
          }
        }
        return _script; 
      }
      set 
      { 
        _script = value;
        _scriptLoaded = true;
      }
    }
    public Func<string> ScriptGetter { get; set; }
  }
}
