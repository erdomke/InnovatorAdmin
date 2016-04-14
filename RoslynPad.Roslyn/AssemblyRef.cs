using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RoslynPad.Roslyn
{
  public class AssemblyRef
  {
    public string Location { get; set; }
    public IEnumerable<string> Namespaces { get; set; }

    public static AssemblyRef FromType(Type type)
    {
      return new AssemblyRef()
      {
        Location = type.Assembly.Location,
        Namespaces = new[] { type.Namespace }
      };
    }
  }
}
