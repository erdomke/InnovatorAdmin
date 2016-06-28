using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  class Logical : AmlElement, ILogical
  {
    public Logical(ElementFactory amlContext, string name, params object[] content) : base(amlContext, name, content) { }
    public Logical(IElement parent, string name) : base(parent, name) { }
    public Logical(IElement parent, IReadOnlyElement elem) : base(parent, elem) { }
  }
}
