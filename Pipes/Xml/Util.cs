using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Linq;
using System.Diagnostics;
using Pipes.Data;

namespace Pipes.Xml
{
  public static class Util
  {
    public static IEnumerable<IXmlElement> Elements(this IEnumerable<IXmlElement> elements, string localName)
    {
      foreach (var elem in elements)
      {
        foreach (var child in elem.Elements(localName))
        {
          yield return child;
        }
      }
    }

    public static IEnumerable<IXmlElement> Descendants(this IEnumerable<IXmlNode> reader, string localName)
    {
      return Descendants(reader, new XmlName(localName));
    }
    public static IEnumerable<IXmlElement> Descendants(this IEnumerable<IXmlNode> reader, IXmlName name)
    {
      var enumer = reader.GetEnumerator();
      var inNode = false;
      int depth = 0;
      XElement elem = null;
      XElementWriter writer = null;

      while (enumer.MoveNext())
      {
        if (inNode)
        {
          if (enumer.Current.Name.Equals(name) && depth == 0) 
          {
            yield return new XElementWrapper(elem);
            inNode = false;
          }
          else 
          {
            switch (enumer.Current.Type)
            {
              case XmlNodeType.Element:
                depth++;
                break;
              case XmlNodeType.EndElement:
                depth--;
                break;
            }
            writer.Node(enumer.Current);
          }
        }
        else if (enumer.Current.Name.Equals(name)) 
        {
          if (string.IsNullOrEmpty(enumer.Current.Name.Prefix))
          {
            elem = new XElement(XName.Get(enumer.Current.Name.LocalName, enumer.Current.Name.Namespace));
          }
          else
          {
            elem = new XElement(XName.Get(enumer.Current.Name.LocalName, enumer.Current.Name.Namespace),
                                new XAttribute(XNamespace.Xmlns + enumer.Current.Name.Prefix, enumer.Current.Name.Namespace));
          }
          writer = new XElementWriter(elem);
          inNode = true;
          depth = 0;
        }
      }
    }
    public static IXmlNode GetXmlNode(object data)
    {
      IXmlNode result = GetXmlElement(data, false);
      if (result == null)
      {
        if (data is XNode)
        {
          return new XNodeWrapper((XNode)data);
        }
        else if (data is System.Xml.XmlNode)
        {
          return new XmlNodeWrapper((System.Xml.XmlNode)data);
        }
        else
        {
          throw new NotSupportedException();
        }
      }
      return result;
    }
    public static IXmlElement GetXmlElement(object data)
    {
      return GetXmlElement(data, true);
    }
    private static IXmlElement GetXmlElement(object data, bool throwException)
    {
      if (data is XDocument)
      {
        return new XElementWrapper(((XDocument)data).Root);
      }
      else if (data is XElement)
      {
        return new XElementWrapper((XElement)data);
      }
      else if (data is XmlElement)
      {
        return new XmlElementWrapper((XmlElement)data);
      }
      else if (data is XmlDocument)
      {
        return new XmlElementWrapper(((XmlDocument)data).DocumentElement);
      }
      else if (data is string)
      {
        return new XElementWrapper(XElement.Parse((string)data));
      }
      else
      {
        if (throwException) {
          throw new NotSupportedException();
        }
        else {
          return null;
        }
      }
    }
   
    public static XmlNodeType GetType(System.Xml.XmlNodeType type, bool isEmptyElement)
    {
      switch (type)
      {
        case System.Xml.XmlNodeType.Attribute:
          return XmlNodeType.Attribute;
        case System.Xml.XmlNodeType.CDATA:
          return XmlNodeType.CDATA;
        case System.Xml.XmlNodeType.Comment:
          return XmlNodeType.Comment;
        case System.Xml.XmlNodeType.DocumentType:
          return XmlNodeType.DocumentType;
        case System.Xml.XmlNodeType.Element:
          if (isEmptyElement)
          {
            return XmlNodeType.EmptyElement;
          }
          else
          {
            return XmlNodeType.Element;
          }
        case System.Xml.XmlNodeType.EndElement:
          return XmlNodeType.EndElement;
        case System.Xml.XmlNodeType.Entity:
          return XmlNodeType.Entity;
        case System.Xml.XmlNodeType.Notation:
          return XmlNodeType.Notation;
        case System.Xml.XmlNodeType.SignificantWhitespace:
          return XmlNodeType.SignificantWhiteSpace;
        case System.Xml.XmlNodeType.Text:
          return XmlNodeType.Text;
        case System.Xml.XmlNodeType.Whitespace:
          return XmlNodeType.Whitespace;
        case System.Xml.XmlNodeType.XmlDeclaration:
          return XmlNodeType.XmlDeclaration;
        default:
          return XmlNodeType.Other;
      }
    }
    internal static IXmlNode GetNode(System.Xml.XmlReader reader)
    {
      var node = new XmlNode();
      node.Name = new XmlName() { LocalName = reader.LocalName, Namespace = reader.NamespaceURI, Prefix = reader.Prefix };
      node.Value = reader.Value;
      node.Type = GetType(reader.NodeType, reader.IsEmptyElement);

      var attrCount = reader.AttributeCount;
      IXmlName xmlName;
      for (int i = 0; i < attrCount; i++)
      {
        reader.MoveToAttribute(i);
        xmlName = new XmlName() { LocalName = reader.LocalName, Namespace = reader.NamespaceURI, Prefix = reader.Prefix };
        node.Item(xmlName, reader.Value);
      }
      reader.MoveToElement();

      return node;
    }


    #region "Extensions"
    public static FieldStatus AttributeStatusAnyCase(this IEnumerable<IXmlFieldValue> elem, string localName)
    {
      var attr = elem.FirstOrDefault(a => string.Compare(a.XmlName.LocalName, localName, StringComparison.OrdinalIgnoreCase) == 0);
      if (attr == null)
      {
        return FieldStatus.Undefined;
      }
      else if (attr.Value == null || attr.Value == DBNull.Value)
      {
        return FieldStatus.Null;
      }
      else if (attr.Value == "")
      {
        return FieldStatus.Empty;
      }
      else
      {
        return FieldStatus.FilledIn;
      }
    }
    public static T AttributeAnyCase<T>(this IEnumerable<IXmlFieldValue> item, string localName, T defaultValue)
    {
      var attr = item.FirstOrDefault(a => string.Compare(a.XmlName.LocalName, localName, StringComparison.OrdinalIgnoreCase) == 0);
      if (attr == null || attr.Value == null || attr.Value == "") return defaultValue;
      return (T)attr.Value;
    }
    public static T Attribute<T>(this IAttributeContainer item, string name, T defaultValue)
    {
      if (item.AttributeStatus(name).IsNullOrEmpty())
      {
        return defaultValue;
      }
      else
      {
        return (T)item.Attribute(name);
      }
    }
    public static Sgml.ISgmlWriter Node(this Sgml.ISgmlWriter writer, IXmlNode node)
    {
      switch (node.Type)
      {
        case XmlNodeType.Attribute:
          return writer.Attribute(node.Name.ToString(), node.Value);
        case XmlNodeType.CDATA:
          return writer.Value(node.Value);
        case XmlNodeType.Comment:
          return writer.Comment((node.Value ?? "").ToString());
        case XmlNodeType.Element:
          if (string.IsNullOrEmpty(node.Name.Namespace))
          {
            writer.Element(node.Name.LocalName);
          }
          else
          {
            writer.NsElement(node.Name.Prefix, node.Name.LocalName, node.Name.Namespace);
          }
          foreach (var attr in (IEnumerable<IXmlFieldValue>)node)
          {
            writer.Attribute(attr.XmlName.Prefix, attr.XmlName.LocalName, attr.XmlName.Namespace, attr.Value);
          }
          return writer;
        case XmlNodeType.EmptyElement:
          if (string.IsNullOrEmpty(node.Name.Namespace))
          {
            writer.Element(node.Name.LocalName);
          }
          else
          {
            writer.NsElement(node.Name.Prefix, node.Name.LocalName, node.Name.Namespace);
          }
          foreach (var attr in (IEnumerable<IXmlFieldValue>)node)
          {
            writer.Attribute(attr.XmlName.Prefix, attr.XmlName.LocalName, attr.XmlName.Namespace, attr.Value);
          }
          return writer.ElementEnd();
        case XmlNodeType.EndElement:
          return writer.ElementEnd();
        case XmlNodeType.Text:
          return writer.Value(node.Value);
        case XmlNodeType.Whitespace:
          return writer.Raw((node.Value ?? "").ToString());
        case XmlNodeType.DocumentType:
          return writer.Raw(string.Format("<!DOCTYPE {0}>", node.Name));
        default:
          break;
      }
      return writer;
    }
    public static IEnumerable<IXmlNode> SkipTo(this IEnumerable<IXmlNode> reader, Func<IXmlNode, bool> predicate)
    {
      return reader.SkipWhile(n => !predicate(n));
    }
    public static void WriteTo(this IEnumerable<IXmlNode> source, Sgml.ISgmlWriter writer)
    {
      foreach (var node in source)
      {
        writer.Node(node);
      }
    }
    /// <summary>
    /// Write the contents of an XML element to a writer.  Assumes that the enumerator is 
    /// positioned to the element start node.  Will leave the enumerator positioned to the element
    /// end node
    /// </summary>
    /// <param name="source">Enumerator with the data</param>
    /// <param name="writer">Writer to render the data to</param>
    public static void WriteTo(this IEnumerator<IXmlNode> source, Sgml.ISgmlWriter writer)
    {
      writer.Node(source.Current);
      int openNodes = (source.Current.Type == XmlNodeType.Element ? 1 : 0);
      while (openNodes > 0 && source.MoveNext())
      {
        writer.Node(source.Current);
        openNodes += (source.Current.Type == XmlNodeType.Element ? 1 : (source.Current.Type == XmlNodeType.EndElement ? -1 : 0));
      }
    }
    public static void WriteTo(this IXmlElement elem, Sgml.ISgmlWriter writer)
    {
      if (string.IsNullOrEmpty(elem.Name.Namespace))
      {
        writer.Element(elem.Name.LocalName);
      }
      else
      {
        writer.NsElement(elem.Name.Prefix, elem.Name.LocalName, elem.Name.Namespace);
      }
      foreach (var attr in elem.Attributes)
      {
        writer.Attribute(attr.Name, attr.Value);
      }
      foreach (var child in elem.Elements())
      {
        WriteTo(child, writer);
      }
      if (elem.Value != null)
      {
        writer.Value(elem.Value);
      }
      writer.ElementEnd();
    }
    public static void WriteTo(this IXmlElement elem, Sgml.ISgmlGroupWriter writer)
    {
      if (string.IsNullOrEmpty(elem.Name.Namespace))
      {
        writer.Element(elem.Name.LocalName);
      }
      else
      {
        writer.NsElement(elem.Name.Prefix, elem.Name.LocalName, elem.Name.Namespace);
      }
      foreach (var attr in elem.Attributes)
      {
        writer.Attribute(attr.Name, attr.Value);
      }

      var childNodes = new Dictionary<String, List<IXmlElement>>();
      List<IXmlElement> childList = null;

      foreach (var child in elem.Elements())
      {
        if (!childNodes.TryGetValue(child.Name.ToString(), out childList) || childList == null)
        {
          childList = new List<IXmlElement>();
          childNodes[child.Name.ToString()] = childList;
        }
        childList.Add(child);
      }
      foreach (var kvp in childNodes)
      {
        if (kvp.Value.Count > 1) writer.ElementGroup(kvp.Key);
        foreach (var child in kvp.Value)
        {
          WriteTo(child, writer);
        }
        if (kvp.Value.Count > 1) writer.ElementGroupEnd();
      }
      if (elem.Value != null)
      {
        writer.Value(elem.Value);
      }
      writer.ElementEnd();
    }
    public static IEnumerable<IXmlNode> XPathSelect(this IXmlReader xmlReader, string xpath)
    {
      XPathNodeIterator iter;
      XmlReader pathReader;
      foreach (var reader in xmlReader.GetReaders())
      {
        var doc = new XPathDocument(reader);
        var nav = doc.CreateNavigator();
        iter = nav.Select(xpath);

        while (iter.MoveNext())
        {
          pathReader = iter.Current.ReadSubtree();
          while (pathReader.Read())
          {
            yield return GetNode(pathReader);
          }
        }
      }
    }
    #endregion
  }
}
