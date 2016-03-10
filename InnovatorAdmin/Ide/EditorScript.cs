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
    private Task<string> _script;
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
        if (_script.IsCompleted)
          return _script.Result;
        return null;
      }
      set
      {
        _script = Task.FromResult(value);
      }
    }
    public Func<Task<string>> ScriptGetter { get; set; }
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
    public Task<string> GetScript()
    {
      if (_script == null)
      {
        _script = ScriptGetter.Invoke();
      }
      return _script;
    }

    public static void BuildMenu(ToolStripItemCollection items, IEnumerable<IEditorScript> scripts, Func<IEditorScript, Task> callback)
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
          items.Add(new ToolStripMenuItem(script.Name, null, async (s, ev) =>
          {
            var text = await script.GetScript(); // Trigger execution
            if (!string.IsNullOrEmpty(text))
            {
              await callback(script);
            }
          }));
        }
      }
    }
  }
}
