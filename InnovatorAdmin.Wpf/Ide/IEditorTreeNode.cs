using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public interface IEditorTreeNode
  {
    InnovatorAdmin.Editor.IconInfo Image { get; }
    string Name { get; }
    string Description { get; }
    bool HasChildren { get; }

    IEnumerable<IEditorScript> GetScripts();
    IEnumerable<IEditorTreeNode> GetChildren();
  }
}
