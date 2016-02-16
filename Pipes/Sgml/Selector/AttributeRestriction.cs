using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml.Selector
{
  public class AttributeRestriction : BaseSelector
  {
    private Xml.XmlName _name = new Xml.XmlName();
    private bool _idSpecificity;

    public AttributeComparison Comparison { get; set; }
    public bool IdSpecificity { get { return _idSpecificity; } }
    public Xml.XmlName Name { get { return _name; } }
    public string Value { get; set; }

    public AttributeRestriction() { }
    public AttributeRestriction(string localName)
    {
      _name.LocalName = localName;
    }

    public static AttributeRestriction Id(string id)
    {
      var result = new AttributeRestriction();
      result.Name.LocalName = "id";
      result.Comparison = AttributeComparison.Equals;
      result.Value = id;
      result._idSpecificity = true;
      return result;
    }

    public static AttributeRestriction Class(string className)
    {
      var result = new AttributeRestriction();
      result.Name.LocalName = "class";
      result.Comparison = AttributeComparison.WhitespaceListContains;
      result.Value = className;
      return result;
    }

    public override void Visit(ISelectorVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
