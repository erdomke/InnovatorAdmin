using System;
using System.Collections.Generic;
using System.Text;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  class ItemTypeCollection : Dictionary<string, ItemType>
  {
    public void Add(ItemType item)
    {
      this.Add(item.Name, item);
    }
  }
}
