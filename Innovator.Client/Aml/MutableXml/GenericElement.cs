using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal class GenericElement : Element
  {
    internal GenericElement(ElementFactory factory, string name, params object[] content) : base(factory, name, content) {}
    internal GenericElement(ElementFactory factory, XmlElement node) : base(factory, node) { } 
  }
}
