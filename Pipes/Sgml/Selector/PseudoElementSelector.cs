using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public class PseudoElementSelector : BaseSelector
  {
    public PseudoElements Element { get; set; }

    public PseudoElementSelector(PseudoElements element)
    {
      this.Element = element;
    }

    public override void Visit(ISelectorVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
