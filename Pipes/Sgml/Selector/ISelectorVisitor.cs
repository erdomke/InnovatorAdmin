using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public interface ISelectorVisitor
  {
    void Visit(AllSelector selector);
    void Visit(AggregateSelectorList selector);
    void Visit(AttributeRestriction selector);
    void Visit(ComplexSelector selector);
    void Visit(ElementRestriction selector);
    void Visit(InlineSelector selector);
    void Visit(NthChildSelector selector);
    void Visit(PseudoElementSelector selector);
    void Visit(PseudoFunction selector);
    void Visit(PseudoSelector selector);
    void Visit(UnknownSelector selector);
  }
}
