using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public interface ISelector
  {
    void Visit(ISelectorVisitor visitor);
  }
}
