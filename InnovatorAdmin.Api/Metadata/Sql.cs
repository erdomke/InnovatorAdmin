using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class Sql : ItemReference
  {
    public string Type { get; set; }

    public static Sql FromFullItem(IReadOnlyItem elem, bool getKeyedName)
    {
      var result = new Sql();
      FillItemRef(result, elem, getKeyedName);
      return result;
    }
  }
}
