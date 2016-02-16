using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public class InlineSelector : BaseSelector
  {
    private InlineSelector() { }
    
    public override void Visit(ISelectorVisitor visitor)
    {
      visitor.Visit(this);
    }

    private static InlineSelector _value = new InlineSelector();

    public static InlineSelector Value { get { return _value; }}
  }
}
