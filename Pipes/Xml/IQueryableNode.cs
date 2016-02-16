using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public interface IQueryableNode
  {
    IXmlName Name { get; } 
    bool TryGetAttributeValue(IXmlName name, StringComparison comparison, out string value);
    bool IsEmpty();
    IQueryableNode Parent();
    IQueryableNode PrecedingSibling();
    IQueryableNode FollowingSibling();
  }
}
