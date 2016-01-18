using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal class Logical : Element, ILogical
  {
    internal Logical(ElementFactory factory, string name, params object[] content) : base(factory, name, content) { }
    internal Logical(ElementFactory factory, XmlElement node) : base(factory, node) { }
  }
}
