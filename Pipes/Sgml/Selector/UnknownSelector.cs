using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public class UnknownSelector : BaseSelector
  {
    public string Token { get; set; }

    public UnknownSelector(string token)
    {
      this.Token = token;
    }

    public override void Visit(ISelectorVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
