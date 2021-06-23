using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace InnovatorAdmin.Documentation
{
  [DebuggerDisplay("{Name,nq}")]
  public class OperationElement
  {
    private List<OperationElement> _attributes;
    private List<OperationElement> _elements;
    private List<AmlTypeDefinition> _valueTypes;

    public string OriginalXPath { get; set; }
    public string Name { get; set; }
    public List<IElement> Summary { get; }
    public List<IElement> Documentation { get; } = new List<IElement>();

    public IEnumerable<OperationElement> Attributes { get { return _attributes ?? Enumerable.Empty<OperationElement>(); } }
    public IEnumerable<OperationElement> Elements { get { return _elements ?? Enumerable.Empty<OperationElement>(); } }
    public IEnumerable<AmlTypeDefinition> ValueTypes { get { return _valueTypes ?? Enumerable.Empty<AmlTypeDefinition>(); } }

    private OperationElement()
    {
      Summary = new List<IElement>();
    }

    public OperationElement(string name) : this()
    {
      Name = name;
    }

    public OperationElement(string name, string summary)
    {
      Name = name;
      Summary = new List<IElement>() { new TextRun(summary) };
    }

    public OperationElement(string name, string summary, AmlDataType dataType, params string[] values) : this(name, summary)
    {
      _valueTypes = new List<AmlTypeDefinition>() { AmlTypeDefinition.FromDefinition(dataType, values) };
    }

    public IEnumerable<OperationElement> DescendantDoc()
    {
      foreach (var attribute in Attributes)
        yield return attribute;
      foreach (var element in Elements)
      {
        yield return element;
        foreach (var child in element.DescendantDoc())
          yield return child;
      }
    }

    public OperationElement GetOrAddAttribute(string name)
    {
      _attributes = _attributes ?? new List<OperationElement>();
      var attr = _attributes.FirstOrDefault(d => d.Name == name);
      if (attr == null)
      {
        attr = new OperationElement()
        {
          Name = name
        };
        _attributes.Add(attr);
      }
      return attr;
    }

    public OperationElement GetOrAddElement(string name)
    {
      _elements = _elements ?? new List<OperationElement>();
      var element = _elements.FirstOrDefault(d => d.Name == name);
      if (element == null)
      {
        element = new OperationElement()
        {
          Name = name
        };
        _elements.Add(element);
      }
      return element;
    }

    public OperationElement WithAttributes(IEnumerable<OperationElement> attributes)
    {
      _attributes = _attributes ?? new List<OperationElement>();
      _attributes.AddRange(attributes.Where(a => !_attributes.Any(e => e.Name == a.Name)).ToList());
      return this;
    }

    public OperationElement WithAttribute(string name, string summary, AmlDataType dataType, params string[] values)
    {
      var attr = GetOrAddAttribute(name);
      attr.Summary.Add(new TextRun(summary));
      attr._valueTypes = new List<AmlTypeDefinition>() { AmlTypeDefinition.FromDefinition(dataType, values) };
      return this;
    }

    public OperationElement WithElements(params OperationElement[] elements)
    {
      _elements = _elements ?? new List<OperationElement>();
      _elements.AddRange(elements);
      return this;
    }

    private enum Language
    {
      Unknown,
      Csharp,
      Vb
    }

    public static OperationElement Parse(string methodName, string methodCode)
    {
      if (string.IsNullOrEmpty(methodCode))
        return new OperationElement { Name = methodName };

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
        return new OperationElement { Name = methodName };

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

      return Parse(methodName, elements);
    }

    private static OperationElement Parse(string methodName, IEnumerable<XElement> elements)
    {
      var method = new OperationElement
      {
        Name = methodName
      };
      var builder = new StringBuilder();

      method.Summary.AddRange(elements
        .Where(e => e.Name.LocalName == "summary")
        .SelectMany(e => ParseDoc(e.Nodes())));

      foreach (var elem in elements.Where(e => e.Name.LocalName == "inheritdoc"))
      {
        if (Standard.TryGetValue((string)elem.Attribute("cref"), out var doc))
        {
          method.WithAttributes(doc.Attributes);
          method.WithElements(doc.Elements.ToArray());
          if (method.Summary.Count < 1 && doc.Summary.Count > 0)
            method.Summary.AddRange(doc.Summary);
        }
      }

      foreach (var elem in elements.Where(e => e.Name.LocalName == "param"))
      {
        var xPath = new List<XPathToken>();
        try
        {
          xPath = XPathToken.Parse((string)elem.Attribute("name")).ToList();
        }
        catch (Exception) { }

        var stack = new Stack<OperationElement>();
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

        stack.Peek().OriginalXPath = (string)elem.Attribute("name");
        stack.Peek().Summary.AddRange(ParseDoc(elem.Nodes().Where(n => !(n is XElement e && e.Name.LocalName == "datatype"))));
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

      method.Documentation.AddRange(ParseDoc(elements
        .Where(e => e.Name.LocalName != "summary" && e.Name.LocalName != "param" && e.Name.LocalName != "inheritdoc")));
      return method;
    }

    private static bool IsWhitespace(XNode node)
    {
      return string.IsNullOrWhiteSpace((node as XText)?.Value ?? (node as XCData)?.Value ?? "*");
    }

    internal static IEnumerable<IElement> ParseDoc(IEnumerable<XNode> nodes)
    {
      var list = nodes
        .Where(n => !IsWhitespace(n))
        .ToList();
      for (var i = 0; i < list.Count; i++)
      {
        var node = list[i];
        if (node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.CDATA)
        {
          var text = (node as XText)?.Value ?? (node as XCData)?.Value ?? "";
          if (i == 0)
            text = text.TrimStart();
          if (i == list.Count - 1)
            text = text.TrimEnd();

          yield return new TextRun(Regex.Replace(text, @"\s+", " "));
        }
        else if (node is XElement child)
        {
          switch (child.Name.LocalName)
          {
            case "para":
              yield return new Paragraph(ParseDoc(child.Nodes()));
              break;
            case "c":
            case "b":
            case "strong":
            case "em":
            case "i":
              yield return GetRun(child, RunStyle.Default);
              break;
            case "a":
              yield return new Hyperlink((string)child.Attribute("href"), ParseDoc(child.Nodes()));
              break;
            case "example":
              yield return new Section("Example", ParseDoc(child.Nodes()));
              break;
            case "remarks":
              yield return new Section("Remarks", ParseDoc(child.Nodes()));
              break;
            case "exception":
              yield return new Section("Exception", ParseDoc(child.Nodes()));
              break;
            case "list":
              yield return List.Parse(child);
              break;
            case "see":
              var parts = ((string)child.Attribute("cref") ?? "").Split('.');
              yield return new DocLink() { Type = parts[0], Name = parts.Last() };
              break;
            case "code":
              yield return new CodeBlock() { Code = (string)child, Language = (string)child.Attribute("lang") };
              break;
          }
        }
      }
    }

    private static TextRun GetRun(XNode node, RunStyle style)
    {
      var elem = node as XElement;

      switch (elem?.Name.LocalName ?? "")
      {
        case "c":
          return GetRun(elem.Nodes().FirstOrDefault(), style | RunStyle.Code);
        case "b":
        case "strong":
          return GetRun(elem.Nodes().FirstOrDefault(), style | RunStyle.Bold);
        case "em":
        case "i":
          return GetRun(elem.Nodes().FirstOrDefault(), style | RunStyle.Italic);
      }

      return new TextRun((node as XText)?.Value ?? (node as XCData)?.Value, style);
    }

    public static Dictionary<string, OperationElement> Standard { get; } = new Dictionary<string, OperationElement>();

    static OperationElement() {
      using (var stream = Assembly.GetExecutingAssembly()
        .GetManifestResourceStream("InnovatorAdmin.Documentation.ActionDocumentation.xml"))
      {
        var xml = XElement.Load(stream);
        foreach (var method in xml.Elements("method"))
        {
          Standard[(string)method.Attribute("name")] = Parse((string)method.Attribute("name"), method.Elements());
        }
      }
    }
  }
}
