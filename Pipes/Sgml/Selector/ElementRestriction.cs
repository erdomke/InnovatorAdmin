using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public class ElementRestriction : BaseSelector
  {
    private Xml.XmlName _name = new Xml.XmlName();

    public Xml.XmlName Name { get { return _name; } }

    public ElementRestriction()
    {
      _name.Namespace = "*";
    }

    public static ElementRestriction LocalName(string name)
    {
      var result = new ElementRestriction();
      result.Name.LocalName = name;
      return result;
    }

    public override void Visit(ISelectorVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
