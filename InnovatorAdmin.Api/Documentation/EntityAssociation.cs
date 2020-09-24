using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Documentation
{
  public class EntityAssociation
  {
    public AssociationEnd Source { get; set; }
    public AssociationEnd Related { get; set; }
    public string Label { get; set; }
    public bool Dashed { get; set; }
  }
}
