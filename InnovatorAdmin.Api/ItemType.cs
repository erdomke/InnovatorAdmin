using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin
{
  public class ItemType
  {
    private List<ItemType> _relationships = new List<ItemType>();
    
    public string Id { get; set; }
    public bool IsCore { get; set; }
    public bool IsFederated { get; set; }
    public bool IsPolymorphic { get; set; }
    public bool IsVersionable { get; set; }
    public string Name { get; set; }
    public ItemReference Reference { get; set; }
    public IList<ItemType> Relationships
    {
      get { return _relationships; }
    }

  }
}
