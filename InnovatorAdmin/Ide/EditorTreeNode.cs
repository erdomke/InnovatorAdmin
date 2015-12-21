using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class EditorTreeNode : IEditorTreeNode
  {
    private bool _childrenLoaded;
    private bool _scriptsLoaded;
    private IEnumerable<IEditorTreeNode> _children;
    private IEnumerable<IEditorScript> _scripts;

    public string ImageKey { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool HasChildren { get; set; }
    public IEnumerable<IEditorTreeNode> Children
    {
      get { return _children; }
      set
      {
        _children = value;
        _childrenLoaded = true;
      }
    }
    public Func<IEnumerable<IEditorTreeNode>> ChildGetter { get; set; }
    public IEnumerable<IEditorScript> Scripts
    {
      get { return _scripts; }
      set
      {
        _scripts = value;
        _scriptsLoaded = true;
      }
    }
    public Func<IEnumerable<IEditorScript>> ScriptGetter { get; set; }

    public IEnumerable<IEditorTreeNode> GetChildren()
    {
      if (!_childrenLoaded)
      {
        _childrenLoaded = true;
        if (this.ChildGetter != null)
        {
          Utils.CallWithTimeout(5000, () => _children = this.ChildGetter.Invoke());
        }
      }
      return _children ?? Enumerable.Empty<IEditorTreeNode>();
    }

    public IEnumerable<IEditorScript> GetScripts()
    {
      if (!_scriptsLoaded)
      {
        _scriptsLoaded = true;
        if (this.ScriptGetter != null)
        {
          Utils.CallWithTimeout(5000, () => _scripts = this.ScriptGetter.Invoke());
        }
      }
      return _scripts ?? Enumerable.Empty<IEditorScript>();
    }
  }
}
