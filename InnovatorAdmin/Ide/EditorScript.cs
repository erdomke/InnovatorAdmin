using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public class EditorScript : IEditorScript
  {
    private string _script;
    private bool _scriptLoaded;
    private List<IEditorScript> _children;

    public bool AutoRun { get; set; }
    public IEnumerable<IEditorScript> Children
    {
      get { return _children ?? Enumerable.Empty<IEditorScript>(); }
    }
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
    public OutputType PreferredOutput { get; set; }

    public EditorScript()
    {
      this.AutoRun = false;
    }

    public EditorScript Add(IEditorScript child)
    {
      if (_children == null)
        _children = new List<IEditorScript>();
      _children.Add(child);
      return this;
    }

    public static void BuildMenu(ToolStripItemCollection items, IEnumerable<IEditorScript> scripts, Action<IEditorScript> callback)
    {
      foreach (var script in scripts)
      {
        if (script.Name.StartsWith("----"))
        {
          items.Add(new ToolStripSeparator());
        }
        else if (script.Children.Any())
        {
          var item = new ToolStripMenuItem(script.Name);
          BuildMenu(item.DropDownItems, script.Children, callback);
          items.Add(item);
        }
        else
        {
          items.Add(new ToolStripMenuItem(script.Name, null, (s, ev) =>
          {
            var query = script.Script; // Trigger execution
            if (!string.IsNullOrEmpty(script.Action) && !string.IsNullOrEmpty(query))
            {
              callback(script);
            }
          }));
        }
      }
    }
  }
}
