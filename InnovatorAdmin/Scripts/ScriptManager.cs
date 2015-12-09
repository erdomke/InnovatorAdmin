using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Innovator.Client;
using System.Windows.Forms;
using System.Drawing;
using InnovatorAdmin.Controls;

namespace InnovatorAdmin.Scripts
{
  public class ScriptManager
  {
    private Dictionary<string, Type> _scripts = new Dictionary<string,Type>();

    public IEnumerable<string> ScriptNames
    {
      get { return _scripts.Keys; }
    }

    public string GetName(IAsyncScript script)
    {
      return GetName(script.GetType());
    }
    private string GetName(Type type)
    {
      var nameAttr = type.GetCustomAttribute<DisplayNameAttribute>();
      return nameAttr == null ? type.Name : nameAttr.DisplayName;
    }

    public bool TryGetNewScript(string name, out IAsyncScript script)
    {
      Type type;
      script = null;
      if (_scripts.TryGetValue(name, out type))
      {
        script = FastObjectFactory.CreateObject<IAsyncScript>(type);
        return true;
      }
      return false;
    }

    public IAsyncScript PromptForScript(IWin32Window owner, Rectangle bounds)
    {
      using (var dialog = new FilterSelect<string>())
      {
        dialog.Message = "Select a script";
        dialog.DataSource = _scripts.Keys;
        IAsyncScript script;
        if (dialog.ShowDialog(owner, bounds) == DialogResult.OK &&
          TryGetNewScript(dialog.SelectedItem, out script))
        {
          return script;
        }
      }
      return null;
    }

    private static ScriptManager _instance;

    public static ScriptManager Instance { get { return _instance; } }

    static ScriptManager()
    {
      var assemblies = new List<Assembly>();
      assemblies.Add(Assembly.GetExecutingAssembly());
      _instance = new ScriptManager();

      var types = assemblies
        .SelectMany(a => a.GetTypes())
        .Where(t => typeof(IAsyncScript).IsAssignableFrom(t));
      foreach (var type in types)
      {
        _instance._scripts[_instance.GetName(type)] = type;
      }
    }
  }
}
