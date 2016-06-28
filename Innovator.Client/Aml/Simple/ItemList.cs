using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  class ItemList : List<IReadOnlyItem>, ILink<ItemList>
  {
    public string Name { get; set; }
    public ItemList Next { get; set; }
  }
}
