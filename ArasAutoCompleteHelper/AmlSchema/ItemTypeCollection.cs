using System;
using System.Collections.Generic;
using System.Text;

namespace Aras.AutoComplete.AmlSchema
{
  class ItemTypeCollection : Dictionary<string, ItemType>
  {
    public void Add(ItemType item)
    {
      this.Add(item.Name, item);
    }
  }
}
