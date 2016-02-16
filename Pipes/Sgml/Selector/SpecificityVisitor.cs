using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public class SpecificityVisitor : ISelectorVisitor
  {
    public int Specificity { get; set;}

    public void Visit(AllSelector selector)
    {
      // Do Nothing
    }

    public void Visit(AggregateSelectorList selector)
    {
      foreach (var sel in selector) sel.Visit(this);
    }

    public void Visit(AttributeRestriction selector)
    {
      this.Specificity += (selector.IdSpecificity ? (1 << 12) : (1 << 8));
    }

    public void Visit(ComplexSelector selector)
    {
      foreach (var sel in selector) sel.Selector.Visit(this);
    }

    public void Visit(ElementRestriction selector)
    {
      this.Specificity += (1 << 4);
    }

    public void Visit(NthChildSelector selector)
    {
      this.Specificity += (1 << 8);
    }

    public void Visit(PseudoElementSelector selector)
    {
      this.Specificity += (1 << 4);
    }

    public void Visit(PseudoFunction selector)
    {
      var arg = selector.Body as ISelector;
      if (arg != null) arg.Visit(this);
    }

    public void Visit(PseudoSelector selector)
    {
      this.Specificity += (1 << 8);
    }

    public void Visit(UnknownSelector selector)
    {
      // Do Nothing
    }

    public void Visit(InlineSelector selector)
    {
      this.Specificity += (1 << 16);
    }
  }
}
