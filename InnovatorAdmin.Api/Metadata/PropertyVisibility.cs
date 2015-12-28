using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  [Flags]
  public enum PropertyVisibility
  {
    None = 0,
    MainGrid = 1,
    RelationshipGrid = 2,
    Both = 3
  }
}
