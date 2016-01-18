using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal class Relationships : Element, IRelationships
  {
    internal Relationships(ElementFactory factory, params object[] content) : base(factory, "Relationships", content) { }
    internal Relationships(ElementFactory factory, XmlElement node) : base(factory, node) { }
  }
}
