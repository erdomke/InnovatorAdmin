using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public class PseudoSelector : BaseSelector
  {
    public PseudoTypes Type;

    public PseudoSelector(PseudoTypes type)
    {
      this.Type = type;
    }

    public override void Visit(ISelectorVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
