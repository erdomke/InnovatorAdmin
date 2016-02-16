using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Json;

namespace Pipes.Xml
{
  public class XmlJsonReader : IPipeOutput<IXmlNode>, IPipeInput<IJsonNode>
  {
    IEnumerable<IJsonNode> _source;

    public IEnumerator<IXmlNode> GetEnumerator()
    {
      var jsonNodes = new Stack<IJsonNode>();
      var prefixes = new Dictionary<string, string>();
      XmlNode xmlNode = null;

      foreach (var node in _source)
      {
        switch (node.Type)
        {
          case JsonNodeType.Array:
            if (xmlNode != null)
            {
              yield return xmlNode;
              xmlNode = null;
            }
            if (string.IsNullOrEmpty(node.Name)) throw new InvalidOperationException();
            jsonNodes.Push(node);
            xmlNode = new XmlNode() { Name = GetXmlName(node.Name, prefixes), Type = XmlNodeType.Element };

            break;
          case JsonNodeType.ArrayEnd:
            if (xmlNode != null)
            {
              yield return xmlNode;
              xmlNode = null;
            }
            jsonNodes.Pop();
            break;
          case JsonNodeType.Object:
            if (!string.IsNullOrEmpty(node.Name))
            {
              if (xmlNode != null)
              {
                yield return xmlNode;
                xmlNode = null;
              }

              jsonNodes.Push(node);
              xmlNode = new XmlNode() { Name = GetXmlName(node.Name, prefixes), Type = XmlNodeType.Element };
            }
            break;
          case JsonNodeType.ObjectEnd:
            if (xmlNode != null)
            {
              yield return xmlNode;
              xmlNode = null;
            }
            if (jsonNodes.Count > 0)
            {
              var jsonNode = jsonNodes.Pop();
              if (jsonNode.Type != JsonNodeType.Object) jsonNodes.Push(jsonNode);
              yield return new XmlNode() { Type = XmlNodeType.EndElement, Name = GetXmlName(jsonNode.Name, prefixes) };
            }
            break;
          case JsonNodeType.SimpleProperty:
            if (node.Name[0] == '@')
            {
              if (xmlNode == null && !string.IsNullOrEmpty(jsonNodes.Peek().Name))
                xmlNode = new XmlNode() { Name = GetXmlName(jsonNodes.Peek().Name, prefixes), Type = XmlNodeType.Element };
              var fieldVal = new XmlFieldValue(node.Name.Substring(1), node.Value);

              // Deal with namespaces.
              if (fieldVal.XmlName.Prefix == "xmlns")
              {
                prefixes[fieldVal.XmlName.LocalName] = (string)fieldVal.Value;
                if (xmlNode.Name.Prefix == fieldVal.XmlName.LocalName) ((XmlName)xmlNode.Name).Namespace = (string)fieldVal.Value;
              }

              xmlNode.Item(fieldVal);
            }
            else
            {
              if (xmlNode != null)
              {
                yield return xmlNode;
                xmlNode = null;
              }

              if (node.Name == "#text") {
                yield return new XmlNode() { Type = XmlNodeType.Text, Value = node.Value };
              } else {
                if (node.Value == null)
                {
                  yield return new XmlNode() { Type = XmlNodeType.EmptyElement, Name = GetXmlName(node.Name, prefixes) };
                }
                else
                {
                  yield return new XmlNode() { Type = XmlNodeType.Element, Name = GetXmlName(node.Name, prefixes) };
                  yield return new XmlNode() { Type = XmlNodeType.Text, Value = node.Value };
                  yield return new XmlNode() { Type = XmlNodeType.EndElement, Name = GetXmlName(node.Name, prefixes) };
                }
              }
            }
            break;
          default:
            if (node.Value == null)
            {
              if (xmlNode == null)
              {
                yield return new XmlNode() { Type = XmlNodeType.EmptyElement, Name = GetXmlName(jsonNodes.Peek().Name, prefixes) };
              }
              else
              {
                xmlNode.Type = XmlNodeType.EmptyElement;
                yield return xmlNode;
                xmlNode = null;
              }
            }
            else
            {
              if (xmlNode == null)
              {
                yield return new XmlNode() { Type = XmlNodeType.Element, Name = GetXmlName(jsonNodes.Peek().Name, prefixes) };
              }
              else
              {
                yield return xmlNode;
                xmlNode = null;
              }
              yield return new XmlNode() { Type = XmlNodeType.Text, Value = node.Value };
              yield return new XmlNode() { Type = XmlNodeType.EndElement, Name = GetXmlName(jsonNodes.Peek().Name, prefixes) };
            }
            break;
        }
      }

    }

    IXmlName GetXmlName(string name, Dictionary<string, string> prefixes)
    {
      var result = new XmlName(name);
      string ns;
      if (!string.IsNullOrEmpty(result.Prefix) && prefixes.TryGetValue(result.Prefix, out ns))
      {
        result.Namespace = ns;
      }
      return result;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public void Initialize(IEnumerable<Json.IJsonNode> source)
    {
      _source = source;
    }
  }
}
