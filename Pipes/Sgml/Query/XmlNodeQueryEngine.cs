using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Pipes.Sgml.Query
{
  public class XmlNodeQueryEngine : IQueryEngine<XmlNode>
  {
    public bool IsMatch(XmlNode node, Xml.IXmlName elem, StringComparison comparison)
    {
      return (elem.Namespace == "*" || node.NamespaceURI == elem.Namespace)
        && (string.IsNullOrEmpty(elem.LocalName) || string.Compare(elem.LocalName, node.LocalName, comparison) == 0);
    }

    public bool IsTypeMatch(XmlNode x, XmlNode y, StringComparison comparison)
    {
      return string.Compare(x.LocalName, y.LocalName, comparison) == 0 &&
        ((string.IsNullOrEmpty(x.NamespaceURI) && string.IsNullOrEmpty(y.NamespaceURI)) || x.NamespaceURI == y.NamespaceURI);
    }

    public bool TryGetAttributeValue(XmlNode node, Xml.IXmlName elem, StringComparison comparison, out string value)
    {
      value = null;
      if (node.Attributes == null) return false;
      var attr = (from a in node.Attributes.Cast<XmlAttribute>()
                  where (elem.Namespace == "*"
                         || (string.IsNullOrEmpty(a.NamespaceURI) && string.IsNullOrEmpty(elem.Namespace))
                         || a.NamespaceURI == elem.Namespace)
                    && (string.IsNullOrEmpty(elem.LocalName) || string.Compare(elem.LocalName, a.LocalName, comparison) == 0)
                  select a).SingleOrDefault();
      if (attr == null) return false;
      value = attr.Value;
      return true;
    }
    
    public IEnumerable<XmlNode> Parents(XmlNode node)
    {
      var parent = node.ParentNode;
      while (parent != null && !(parent is XmlDocument))
      {
        yield return parent;
        parent = parent.ParentNode;
      }
    }

    public IEnumerable<XmlNode> PrecedingSiblings(XmlNode node)
    {
      var prev = node.PreviousSibling;
      while (prev != null)
      {
        if (prev is XmlElement) yield return prev;
        prev = prev.PreviousSibling;
      }
    }

    public IEnumerable<XmlNode> FollowingSiblings(XmlNode node)
    {
      var next = node.NextSibling;
      while (next != null)
      {
        if (next is XmlElement) yield return next;
        next = next.NextSibling;
      }
    }

    public bool IsEmpty(XmlNode node)
    {
      return !node.HasChildNodes;
    }
  }
}
