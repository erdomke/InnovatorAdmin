using Mvp.Xml.Common.XPath;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace InnovatorAdmin
{
  public static class XmlUtils
  {
    public static string RemoveComments(string xml)
    {
      if (string.IsNullOrEmpty(xml)) { return ""; }
      try
      {
        using (var reader = new StringReader(xml))
        using (var xReader = XmlReader.Create(reader, new XmlReaderSettings()
        {
          IgnoreComments = true
        }))
        {
          var doc = new XmlDocument();
          doc.Load(xReader);
          return doc.OuterXml;
        }
      }
      catch (XmlException)
      {
        return xml;
      }
    }

    public static XmlDocument NewDoc(this XmlNode node)
    {
      var doc = (node as XmlDocument) ?? node.OwnerDocument;
      return new XmlDocument(doc == null ? new NameTable() : doc.NameTable);
    }

    public static IEnumerable<XmlElement> RootItems(XmlElement elem)
    {
      var curr = elem;
      while (curr != null && curr.LocalName != "Item")
        curr = curr.ChildNodes.OfType<XmlElement>().FirstOrDefault();

      if (curr?.LocalName == "Item" && curr.ParentNode != null)
      {
        foreach (var item in curr.ParentNode.Elements("Item"))
          yield return item;
      }
      else if ((curr ?? elem)?.LocalName == "Item")
      {
        yield return curr ?? elem;
      }
    }

    public static XmlElement Elem(this XmlNode node, string localName)
    {
      if (node == null) return null;
      var doc = node as XmlDocument;
      var newElem = (doc ?? node.OwnerDocument).CreateElement(localName);
      node.AppendChild(newElem);
      return newElem;
    }

    public static void Elem(this XmlNode node, string localName, string value)
    {
      if (node == null) return;
      node.Elem(localName).AppendChild(node.OwnerDocument.CreateTextNode(value));
    }

    public static string Attribute(this XmlNode elem, string localName, string defaultValue = null)
    {
      if (elem == null || elem.Attributes == null) return defaultValue;
      var attr = elem.Attributes[localName];
      if (attr == null)
      {
        return defaultValue;
      }
      else
      {
        return attr.Value;
      }
    }

    public static XmlElement Attr(this XmlElement elem, string localName, string value)
    {
      if (elem == null) return elem;
      elem.SetAttribute(localName, value);
      return elem;
    }
    public static void Detach(this XmlNode node)
    {
      if (node != null && node.ParentNode != null)
      {
        var attr = node as XmlAttribute;
        if (attr == null)
        {
          node.ParentNode.RemoveChild(node);
        }
        else
        {
          ((XmlElement)node.ParentNode).RemoveAttributeNode(attr);
        }
      }
    }

    public static XmlElement Element(this XmlNode node, string localName)
    {
      if (node == null) return null;
      return node.ChildNodes.OfType<XmlElement>().SingleOrDefault(e => e.LocalName == localName);
    }

    public static string Element(this XmlNode node, string localName, string defaultValue)
    {
      if (node == null) return defaultValue;
      var elem = node.Element(localName);
      if (elem == null) return defaultValue;
      return elem.InnerText;
    }

    public static XmlElement Element(this IEnumerable<XmlElement> nodes, string localName)
    {
      if (nodes == null) return null;
      return nodes.SelectMany(n => n.ChildNodes.OfType<XmlElement>()).SingleOrDefault(e => e.LocalName == localName);
    }

    public static void VisitDescendantsAndSelf(this XmlNode node, Action<XmlElement> action)
    {
      var elem = node as XmlElement;
      if (elem != null) action.Invoke(elem);
      ElementRecursion(node, action);
    }

    private static void ElementRecursion(XmlNode node, Action<XmlElement> action)
    {
      foreach (var elem in node.Elements())
      {
        action.Invoke(elem);
        ElementRecursion(elem, action);
      }
    }

    public static IEnumerable<XmlElement> Descendants(this XmlNode node, Func<XmlElement, bool> predicate)
    {
      var results = new List<XmlElement>();
      ElementQuery(node, results, predicate);
      return results;
    }

    public static IEnumerable<XmlElement> DescendantsAndSelf(this XmlNode node, Func<XmlElement, bool> predicate)
    {
      var results = new List<XmlElement>();
      if (predicate.Invoke((XmlElement)node)) results.Add((XmlElement)node);
      ElementQuery(node, results, predicate);
      return results;
    }

    private static void ElementQuery(XmlNode node, List<XmlElement> results, Func<XmlElement, bool> predicate)
    {
      foreach (var elem in node.Elements())
      {
        if (predicate.Invoke(elem)) results.Add(elem);
        ElementQuery(elem, results, predicate);
      }
    }

    public static IEnumerable<XmlElement> Elements(this XmlNode node)
    {
      return node.ChildNodes.OfType<XmlElement>();
    }

    public static IEnumerable<XmlElement> Elements(this XmlNode node, string localName)
    {
      return node.ChildNodes.OfType<XmlElement>().Where(e => e.LocalName == localName);
    }

    public static IEnumerable<XmlElement> Elements(this XmlNode node, Func<XmlElement, bool> predicate)
    {
      return node.ChildNodes.OfType<XmlElement>().Where(predicate);
    }

    public static IEnumerable<XmlElement> Elements(this IEnumerable<XmlElement> nodes, Func<XmlElement, bool> predicate)
    {
      return nodes.SelectMany(n => n.ChildNodes.OfType<XmlElement>()).Where(predicate);
    }

    public static IEnumerable<XmlElement> Elements(this IEnumerable<XmlElement> nodes, string localName)
    {
      return nodes.SelectMany(n => n.ChildNodes.OfType<XmlElement>()).Where(e => e.LocalName == localName);
    }

    public static IEnumerable<XmlElement> ElementsByXPath(this XmlNode node, string xPath)
    {
      try
      {
        return XPathCache.SelectNodes(xPath, node).OfType<XmlElement>();
      }
      catch (NullReferenceException)
      {
        return node.SelectNodes(xPath).OfType<XmlElement>();
      }
    }

    public static IEnumerable<XmlElement> ElementsByXPath(this XmlNode node, string xPath, params object[] vars)
    {
      return XPathCache.SelectNodes(xPath, node,
          vars.Select((v, i) => new XPathVariable("p" + i.ToString(), v)).ToArray())
        .OfType<XmlElement>();
    }

    public static bool HasValue(this XmlNode node)
    {
      return node != null && (node.Elements().Any() || !string.IsNullOrEmpty(node.InnerText));
    }

    public static IEnumerable<XmlNode> XPath(this XmlNode node, string xPath)
    {
      return XPathCache.SelectNodes(xPath, node).OfType<XmlNode>();
    }

    public static IEnumerable<XmlNode> XPath(this XmlNode node, string xPath, params object[] vars)
    {
      return XPathCache.SelectNodes(xPath, node,
          vars.Select((v, i) => new XPathVariable("p" + i.ToString(), v)).ToArray())
        .OfType<XmlNode>();
    }

    public static XmlNode Parent(this XmlNode node)
    {
      if (node == null) return null;
      return node.ParentNode;
    }

    public static IEnumerable<XmlElement> Parents(this XmlNode node)
    {
      if (node == null) yield break;

      var parent = node.ParentNode as XmlElement;
      while (parent != null)
      {
        yield return parent;
        parent = parent.ParentNode as XmlElement;
      }
    }

    public static XmlDocument DocFromXml(string xml)
    {
      var result = new XmlDocument();
      result.LoadXml(xml);
      return result;
    }

    public static IEnumerable<XmlElement> NextSiblingsAndSelf(this XmlNode element)
    {
      var curr = element;
      while (curr != null)
      {
        if (curr is XmlElement result)
          yield return result;
        curr = curr.NextSibling;
      }
    }
  }
}
