using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public interface IItemFactory
  {
    Item NewItem(ElementFactory factory, string type);
  }
}
