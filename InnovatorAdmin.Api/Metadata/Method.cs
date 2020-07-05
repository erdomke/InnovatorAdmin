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
    public bool IsCore { get; }
    public AmlDocumentation Documentation { get; }

    public Method(IReadOnlyItem elem, bool isCore = false)
    {
      FillItemRef(this, elem, false);
      this.KeyedName = elem.Property("name").AsString("");
      this.IsCore = elem.Property("core").AsBoolean(isCore);
      this.Documentation = AmlDocumentation.Parse(this.KeyedName, elem.Property("method_code").AsString(""));
      this.Documentation.Summary = this.Documentation.Summary ?? elem.Property("comments").AsString(null);
    }
  }
}
