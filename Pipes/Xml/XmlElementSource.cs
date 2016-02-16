using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public class XmlElementSource : IPipeOutput<IXmlElement>
  {
    private IEnumerable<IXmlElement> _elems;

    public XmlElementSource(object value)
    {
      _elems = new Collections.SingleItemEnumerable<IXmlElement>(Util.GetXmlElement(value));
    }
    public XmlElementSource(IEnumerable<IXmlElement> values)
    {
      _elems = values;
    }

    public IEnumerator<IXmlElement> GetEnumerator()
    {
      foreach (var elem in _elems)
      {
        yield return elem;
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
