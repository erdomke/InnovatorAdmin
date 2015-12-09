using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class Method : ItemReference
  {
    public bool IsCore { get; set; }

    public static Method FromFullItem(IReadOnlyItem elem, bool getKeyedName)
    {
      var result = new Method();
      FillItemRef(result, elem, getKeyedName);
      return result;
    }
  }
}
