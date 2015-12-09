using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class EditorTreeNode : IEditorTreeNode
  {
    private bool _childrenLoaded;
    private IEnumerable<IEditorTreeNode> _children;

    public string ImageKey { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool HasChildren { get; set; }
    public Func<IEnumerable<IEditorTreeNode>> ChildGetter { get; set; }
    public IEnumerable<EditorScript> Scripts { get; set; }

    public EditorTreeNode()
    {
      this.Scripts = Enumerable.Empty<EditorScript>();
    }

    public IEnumerable<IEditorTreeNode> GetChildren()
    {
      if (!_childrenLoaded)
      {
        _childrenLoaded = true;
        _children = ChildGetter.Invoke();
      }
      return _children;
    }
  }
}
