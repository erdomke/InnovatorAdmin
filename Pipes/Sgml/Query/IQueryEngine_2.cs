using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Sgml.Selector;
using Pipes.Xml;

namespace Pipes.Sgml.Query
{
  public interface IQueryEngine<T>
  {
    bool IsEmpty(T node);
    bool IsMatch(T node, IXmlName selector, StringComparison comparison);
    bool IsTypeMatch(T x, T y, StringComparison comparison);
    bool TryGetAttributeValue(T node, IXmlName selector, StringComparison comparison, out string value);
    IEnumerable<T> Parents(T node);
    IEnumerable<T> PrecedingSiblings(T node);
    IEnumerable<T> FollowingSiblings(T node);
  }
}
