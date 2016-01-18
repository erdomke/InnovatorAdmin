using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal static class XmlUtils
  {
    public static StringBuilder AppendEscapedXml(this StringBuilder builder, string value)
    {
      if (value == null) return builder;
      builder.EnsureCapacity(builder.Length + value.Length + 10);
      for (var i = 0; i < value.Length; i++)
      {
        switch (value[i])
        {
          case '&':
            builder.Append("&amp;");
            break;
          case '<':
            builder.Append("&lt;");
            break;
          case '>':
            builder.Append("&gt;");
            break;
          case '"':
            builder.Append("&quot;");
            break;
          case '\'':
            builder.Append("&apos;");
            break;
          default:
            builder.Append(value[i]);
            break;
        }
      }
      return builder;
    }
    
    public static XmlNode GetLocalNode(this XmlNode local, XmlNode value)
    {
      var doc = (local as XmlDocument) ?? local.OwnerDocument;
      if (value.OwnerDocument == doc)
      {
        return value;
      }
      return doc.ImportNode(value, true);
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
    public static void Detatch(this XmlNode node)
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

    public static void VisitNodes(string data, Func<IEnumerable<string>, XmlReader, bool> callback)
    {
      XmlNameTable table = null;
      VisitNodes(data, ref table, callback);
    }
    public static void VisitNodes(string data, ref XmlNameTable nameTable, Func<IEnumerable<string>, XmlReader, bool> callback)
    {
      XmlTextReader reader;
      if (nameTable == null)
      {
        reader = new XmlTextReader(new StringReader(data)) { WhitespaceHandling = WhitespaceHandling.Significant };
        nameTable = reader.NameTable;
      }
      else
      {
        reader = new XmlTextReader(new StringReader(data), nameTable) { WhitespaceHandling = WhitespaceHandling.Significant };
      }

      var path = new Stack<string>();
      using (reader)
      {
        while (reader.Read())
        {
          switch (reader.NodeType)
          {
            case XmlNodeType.Element:
              path.Push(reader.LocalName);
              break;
            case XmlNodeType.EndElement:
              path.Pop();
              break;
          }

          if (!callback.Invoke(path.Reverse(), reader)) return;
          if (reader.NodeType == XmlNodeType.Element && reader.IsEmptyElement) path.Pop();
        }
      }
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
      return XPathCache.SelectNodes(xPath, node).OfType<XmlElement>();
    }

    public static IEnumerable<XmlElement> ElementsByXPath(this XmlNode node, string xPath, params object[] vars)
    {
      return XPathCache.SelectNodes(xPath, node,
          vars.Select((v, i) => new XPathVariable("p" + i.ToString(), v)).ToArray())
        .OfType<XmlElement>();
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
  }
}
