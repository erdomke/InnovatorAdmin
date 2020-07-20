using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin
{
  [DebuggerDisplay("{Name,nq}")]
  public class AmlDocumentation
  {
    private List<AmlDocumentation> _attributes;
    private List<AmlDocumentation> _elements;
    private List<AmlTypeDefinition> _valueTypes;

    public string Name { get; set; }
    public string Summary { get; set; }

    public IEnumerable<AmlDocumentation> Attributes { get { return _attributes ?? Enumerable.Empty<AmlDocumentation>(); } }
    public IEnumerable<AmlDocumentation> Elements { get { return _elements ?? Enumerable.Empty<AmlDocumentation>(); } }
    public IEnumerable<AmlTypeDefinition> ValueTypes { get { return _valueTypes ?? Enumerable.Empty<AmlTypeDefinition>(); } }

    private AmlDocumentation() { }

    public AmlDocumentation(string name)
    {
      Name = name;
    }

    public AmlDocumentation(string name, string summary)
    {
      Name = name;
      Summary = summary;
    }

    public AmlDocumentation(string name, string summary, AmlDataType dataType, params string[] values) : this(name, summary)
    {
      _valueTypes = new List<AmlTypeDefinition>() { AmlTypeDefinition.FromDefinition(dataType, values) };
    }

    

    public AmlDocumentation GetOrAddAttribute(string name)
    {
      _attributes = _attributes ?? new List<AmlDocumentation>();
      var attr = _attributes.FirstOrDefault(d => d.Name == name);
      if (attr == null)
      {
        attr = new AmlDocumentation()
        {
          Name = name
        };
        _attributes.Add(attr);
      }
      return attr;
    }

    public AmlDocumentation GetOrAddElement(string name)
    {
      _elements = _elements ?? new List<AmlDocumentation>();
      var element = _elements.FirstOrDefault(d => d.Name == name);
      if (element == null)
      {
        element = new AmlDocumentation()
        {
          Name = name
        };
        _elements.Add(element);
      }
      return element;
    }

    public AmlDocumentation WithAttributes(IEnumerable<AmlDocumentation> attributes)
    {
      _attributes = _attributes ?? new List<AmlDocumentation>();
      _attributes.AddRange(attributes.Where(a => !_attributes.Any(e => e.Name == a.Name)).ToList());
      return this;
    }

    public AmlDocumentation WithAttribute(string name, string summary, AmlDataType dataType, params string[] values)
    {
      var attr = GetOrAddAttribute(name);
      attr.Summary = summary;
      attr._valueTypes = new List<AmlTypeDefinition>() { AmlTypeDefinition.FromDefinition(dataType, values) };
      return this;
    }

    public AmlDocumentation WithElements(params AmlDocumentation[] elements)
    {
      _elements = _elements ?? new List<AmlDocumentation>();
      _elements.AddRange(elements);
      return this;
    }

    private enum Language
    {
      Unknown,
      Csharp,
      Vb
    }

    public static AmlDocumentation Parse(string methodName, string methodCode)
    {
      var method = new AmlDocumentation
      {
        Name = methodName
      };
      if (string.IsNullOrEmpty(methodCode))
        return method;

      var lines = methodCode
        .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
      var xmlComments = new StringBuilder();
      var language = Language.Unknown;

      for (var i = 0; i < lines.Length; i++)
      {
        var trimmed = lines[i].TrimStart();
        if (trimmed.StartsWith("///") || trimmed.StartsWith("'''"))
        {
          if (language == Language.Unknown)
            language = trimmed.StartsWith("///") ? Language.Csharp : Language.Vb;
          else if (language == Language.Csharp && !trimmed.StartsWith("///"))
            break;
          else if (language == Language.Vb && !trimmed.StartsWith("'''"))
            break;
          xmlComments.AppendLine(trimmed.Substring(3));
        }
        else
        {
          break;
        }
      }

      if (xmlComments.Length < 1)
        return method;

      var elements = new List<XElement>();
      using (var reader = new StringReader(xmlComments.ToString()))
      using (var xml = XmlReader.Create(reader, new XmlReaderSettings()
      {
        ConformanceLevel = ConformanceLevel.Fragment
      }))
      {
        try
        {
          xml.MoveToContent();
          while (!xml.EOF)
          {
            if (xml.NodeType == XmlNodeType.Element)
              elements.Add((XElement)XElement.ReadFrom(xml));
            else
              xml.Read();
          }
        }
        catch (XmlException) { }
      }

      var builder = new StringBuilder();
      foreach (var elem in elements.Where(e => e.Name.LocalName == "summary"))
        BuildDocString(elem, builder);
      method.Summary = builder.ToString().Trim();

      foreach (var elem in elements.Where(e => e.Name.LocalName == "param"))
      {
        var xPath = new List<XPathToken>();
        try
        {
          xPath = XPathToken.Parse((string)elem.Attribute("name")).ToList();
        }
        catch (Exception) { }

        var stack = new Stack<AmlDocumentation>();
        stack.Push(method);
        var idx = 0;

        while (idx < xPath.Count)
        {
          if (xPath[idx].TryGetAxis(out var axis) && axis == XPathAxis.Attribute && idx + 1 < xPath.Count)
          {
            var attrDoc = stack.Peek().GetOrAddAttribute(xPath[idx + 1].Value);
            stack.Push(attrDoc);
            idx++;
          }
          else if (xPath[idx].TryGetNameTest(out var elemName) && XPathToken.IsNcName(elemName))
          {
            var elemDoc = stack.Peek().GetOrAddElement(elemName);
            stack.Push(elemDoc);
          }
          else if (xPath[idx].Type == XPathTokenType.Operator
            && xPath[idx].Value == "="
            && idx + 1 < xPath.Count
            && xPath[idx + 1].TryGetString(out var constant))
          {
            if (stack.Peek()._valueTypes == null)
            {
              stack.Peek()._valueTypes = new List<AmlTypeDefinition>()
              {
                AmlTypeDefinition.FromConstants(constant)
              };
            }
            else
            {
              stack.Peek()._valueTypes[0] = AmlTypeDefinition.FromConstants(stack.Peek()._valueTypes[0].Values.Concat(new[] { constant }).ToArray());
            }
            idx++;
          }
          else if ((xPath[idx].Type == XPathTokenType.Separator && xPath[idx].Value == "]")
            || (xPath[idx].Type == XPathTokenType.Operator && xPath[idx].Value == "or"))
          {
            stack.Pop();
          }
          idx++;
        }

        stack.Peek().Summary = BuildDocString(elem);
        foreach (var dataType in elem.Elements("datatype")
          .Where(e => !string.IsNullOrEmpty((string)e.Attribute("type"))))
        {
          var path = (string)dataType.Attribute("path") ?? ".";
          var typeDefns = AmlTypeDefinition.FromType((string)dataType.Attribute("type"));

          if (path == "." || path == "text()")
          {
            stack.Peek()._valueTypes = typeDefns;
          }
          else if (path.StartsWith("@"))
          {
            var attr = stack.Peek().Attributes.FirstOrDefault(d => d.Name == path.Substring(1));
            if (attr == null && stack.Peek().Name == path.Substring(1))
              attr = stack.Peek();
            if (attr != null)
            {
              attr._valueTypes = typeDefns;
            }
          }
        }
      }

      return method;
    }

    private static string BuildDocString(XElement element)
    {
      var builder = new StringBuilder();
      BuildDocString(element, builder);
      return builder.ToString().Trim();
    }

    private static void BuildDocString(XElement element, StringBuilder builder)
    {
      foreach (var node in element.Nodes())
      {
        if (node is XText text)
        {
          builder.Append(Regex.Replace(text.Value, @"\s+", " "));
        }
        else if (node is XCData cData)
        {
          builder.Append(Regex.Replace(cData.Value, @"\s+", " "));
        }
        else if (node is XElement child)
        {
          switch (child.Name.LocalName)
          {
            case "datatype":
              break;
            case "para":
              if (builder.Length > 0)
                builder.Append("\r\n\r\n");
              BuildDocString(child, builder);
              break;
            case "see":
              var cref = (string)child.Attribute("cref");
              if (!string.IsNullOrEmpty(cref))
                builder.Append(cref.Split('.').Last());
              break;
            case "list":
              foreach (var item in child.Elements("item"))
              {
                if (builder.Length > 0)
                  builder.Append("\r\n");
                builder.Append("- ");
                builder.Append(string.Join(": ", new[] {
                  (string)item.Element("term"),
                  (string)item.Element("description")
                }.Where(v => !string.IsNullOrEmpty(v))));
              }
              break;
            default:
              BuildDocString(child, builder);
              break;
          }
        }
      }
    }
  }
}
