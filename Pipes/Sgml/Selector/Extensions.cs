using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public static class Extensions
  {
    public static int GetSpecificity(this ISelector selector)
    {
      var visitor = new SpecificityVisitor();
      selector.Visit(visitor);
      return visitor.Specificity;
    }
  }
}
