using System;
using System.Collections.Generic;
using System.Text;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  class ItemTypeCollection : Dictionary<string, ItemType>
  {
    public ItemTypeCollection() : base(StringComparer.OrdinalIgnoreCase) { }

    public void Add(ItemType item)
    {
      this.Add(item.Name, item);
    }
  }
}
