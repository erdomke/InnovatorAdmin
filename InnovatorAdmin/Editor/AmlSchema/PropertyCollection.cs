using System;
using System.Collections.Generic;
using System.Text;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  class PropertyCollection : Dictionary<string, Property>
  {
    public void Add(Property item)
    {
      this.Add(item.Name, item);
    }
  }
}
