using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public class PseudoFunction : PseudoSelector
  {
    public object Body { get; set; }

    public PseudoFunction(PseudoTypes type) : base(type) { }

    public override void Visit(ISelectorVisitor visitor)
    {
      base.Visit(visitor);
    }
  }
}
