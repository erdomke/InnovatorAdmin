using System;
using System.Diagnostics.Tracing;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin
{
  internal class DiffAnnotation
  {
    public const string OriginalXmlAttribute = "orig_xml";

    private bool _textDiffers;

    public XElement Original { get; }

    internal DiffAnnotation(XElement element, bool textDiffers)
    {
      Original = element;
      _textDiffers = textDiffers;
    }

    public static string ToString(XElement element
      , SaveOptions options = SaveOptions.None
      , DiffVersion version = DiffVersion.Target)
    {
      using (var str = new StringWriter())
      using (var writer = XmlWriter.Create(str, new XmlWriterSettings()
      {
        OmitXmlDeclaration = true,
        Indent = (options & SaveOptions.DisableFormatting) == 0,
        IndentChars = "  "
      }))
      {
        WriteTo(element, writer, version);
        writer.Flush();
        return str.ToString();
      }
    }

    public static void WriteTo(XElement element, XmlWriter writer, DiffVersion version)
    {
      var diff = element.Annotation<DiffAnnotation>();
      if (diff != null)
      {
        if (version == DiffVersion.Original)
        {
          element = diff.Original;
          if (element == null)
            return;
        }
      }

      writer.WriteStartElement(element.Name.LocalName, element.Name.NamespaceName);
      foreach (var attr in element.Attributes())
      {
        writer.WriteStartAttribute(attr.Name.LocalName, attr.Name.NamespaceName);
        writer.WriteString(attr.Value);
        writer.WriteEndAttribute();
      }
      if (diff != null && version == DiffVersion.Both)
      {
        if (diff._textDiffers
          && (diff.Original == null || (string)diff.Original.Attribute("is_null") == "1"))
          writer.WriteAttributeString("orig_is_null", "1");
        else if (diff._textDiffers)
          writer.WriteAttributeString("orig_value", (string)diff.Original);
        else
          writer.WriteAttributeString(OriginalXmlAttribute, diff.Original.ToString());
      }

      foreach (var node in element.Nodes())
      {
        if (node is XCData cData)
          writer.WriteCData(cData.Value);
        else if (node is XText text)
          writer.WriteString(text.Value);
        else if (node is XElement child)
          WriteTo(child, writer, version);
        else if (node is XComment comment)
          writer.WriteComment(comment.Value);
        else
          throw new NotSupportedException($"Node type {node.GetType().Name} is not supported.");
      }
      writer.WriteEndElement();
    }
  }

  public enum DiffVersion
  {
    Target,
    Original,
    Both
  }
}
