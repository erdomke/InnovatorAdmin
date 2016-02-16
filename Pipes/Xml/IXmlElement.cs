using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public interface IXmlElement : IXmlNode, IAttributeContainer
  {
    void Attribute(string name, string value);
    bool HasElements { get; }
    IXmlElement Parent { get; }
    IXmlElement Element(string name);
    IXmlElement Element(int index);
    IEnumerable<IXmlElement> Elements();
    IEnumerable<IXmlElement> Elements(string name);
    IEnumerable<IXmlNode> Nodes();
    new object Value { get; set; }
    IXmlElement XPathSelectElement(string xpath);
    IEnumerable<IXmlElement> XPathSelectElements(string xpath);
  }
}
