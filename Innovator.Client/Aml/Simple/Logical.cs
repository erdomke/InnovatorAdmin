using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Aml.Simple
{
  class Logical : GenericElement, ILogical
  {
    public Logical(string name, IElement parent) : base(name, parent) { }
    public Logical(IReadOnlyElement elem, IElement parent) : base(elem, parent) { }
  }
}
