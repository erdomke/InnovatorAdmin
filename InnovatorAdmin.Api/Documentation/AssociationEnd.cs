using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public class AssociationEnd
  {
    public Entity Entity { get; set; }
    public AssociationType Type { get; set; }
    public int? Multiplicity { get; set; }
    public string Label { get; set; }
  }
}
