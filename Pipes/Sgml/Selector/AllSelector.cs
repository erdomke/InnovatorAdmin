using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public class AllSelector : BaseSelector
  {
    private AllSelector() { }

    private static AllSelector _all = new AllSelector();
    public static AllSelector All()
    {
      return _all;
    }

    public override void Visit(ISelectorVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
