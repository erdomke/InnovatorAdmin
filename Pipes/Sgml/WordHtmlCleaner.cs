using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Data;
using Pipes.Xml;

namespace Pipes.Sgml
{
  public class WordHtmlCleaner : IPipeInput<Xml.IXmlNode>, IPipeOutput<Xml.IXmlNode>
  {
    private IEnumerable<Xml.IXmlNode> _source;

    public void Initialize(IEnumerable<Xml.IXmlNode> source)
    {
      _source = source;
    }

    // TODO: convert list paragraphs to a real list

    public IEnumerator<Xml.IXmlNode> GetEnumerator()
    {
      bool isWord = false;
      bool allowedType;
      bool allowedNamespace;
      bool allowedTag;

      foreach (var node in _source)
      {
        if (node.Name.LocalName.IsEqualIgnoreCase("meta") && (
          (node.Assert<string>("name", a => a == "ProgId") && node.Assert<string>("content", a => a == "Word.Document")) ||
          (node.Assert<string>("name", a => a == "Generator") && node.Assert<string>("content", a => a.StartsWith("Microsoft Word"))) ||
          (node.Assert<string>("name", a => a == "Originator") && node.Assert<string>("content", a => a.StartsWith("Microsoft Word")))
        ))
        {
          isWord = true;
        }

        // Only allow certain types of nodes
        allowedTag = true;
        switch (node.Type)
        {
          case Xml.XmlNodeType.Element:
          case Xml.XmlNodeType.EmptyElement:
          case Xml.XmlNodeType.EndElement:
            switch (node.Name.LocalName)
            {
              case "meta":
              case "link":
              case "style":
                allowedTag = false;
                break;
            }
            allowedType = true;
            break;
          case Xml.XmlNodeType.Attribute:
          case Xml.XmlNodeType.Entity:
          case Xml.XmlNodeType.SignificantWhiteSpace:
          case Xml.XmlNodeType.Text:
          case Xml.XmlNodeType.Whitespace:
            allowedType = true;
            break;
          default:
            allowedType = false;
            break;
        }

        // Ignore MS namespaces
        switch (node.Name.Namespace)
        {
          case "urn:schemas-microsoft-com:vml":
          case "urn:schemas-microsoft-com:office:office":
          case "urn:schemas-microsoft-com:office:word":
          case "http://schemas.microsoft.com/office/2004/12/omml":
            allowedNamespace = false;
            break;
          default:
            allowedNamespace = true;
            break;
        }

        if (!isWord || (allowedNamespace && allowedTag && allowedType))
        {
          if (node.Type == XmlNodeType.Element && node.Cast<IXmlFieldValue>().Any(a => !NamespaceAllowed(a.XmlName.Namespace)))
          {
            yield return new XmlNode(node.Name, node.Cast<IXmlFieldValue>().Where(a => NamespaceAllowed(a.XmlName.Namespace)));
          }
          else
          {
            yield return node;
          }
        }
      }
    }

    private bool NamespaceAllowed(string ns)
    {
      switch (ns)
      {
        case "urn:schemas-microsoft-com:vml":
        case "urn:schemas-microsoft-com:office:office":
        case "urn:schemas-microsoft-com:office:word":
        case "http://schemas.microsoft.com/office/2004/12/omml":
          return false;
        default:
          return true;
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

  }
}
