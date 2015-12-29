using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace InnovatorAdmin.Editor
{
  public class XmlFragmentReader : XmlReader, IXmlLineInfo
  {
    const string __noName = "___NO_NAME___";
    const string __eof = "{`EOF`}";

    private StringBuilder _builder = new StringBuilder();
    private TextReader _reader;
    private int _i;
    private XmlReader _xml;
    private List<int> _lineOffsets = new List<int>() { 0 };


    public int Offset
    {
      get
      {
        var lineInfo = (IXmlLineInfo)_xml;
        return _lineOffsets[lineInfo.LineNumber - 1]
          + lineInfo.LinePosition
          - (_xml.NodeType == XmlNodeType.Element ? 2 : 1);
      }
    }

    public XmlFragmentReader(TextReader reader)
    {
      _reader = reader;
      Process();
    }

    private void Process()
    {
      var state = XmlState.Other;
      var line = 1;

      var ch = _reader.Read();
      _i = 0;
      while (ch > 0)
      {
        switch (ch)
        {
          case '\r':
            ReadText('\n');
            line++;
            _lineOffsets.Add(_i + 1);
            if (state == XmlState.Tag) state = XmlState.Attribute;
            break;
          case '\n':
            line++;
            _lineOffsets.Add(_i + 1);
            if (state == XmlState.Tag) state = XmlState.Attribute;
            break;
          default:
            switch (state)
            {
              case XmlState.Attribute:
                if (ch == '=')
                {
                  if (ReadText('"') || ReadText('\''))
                  {
                    state = XmlState.AttributeValue;
                  }
                }
                else if (ch == '>')
                {
                  state = XmlState.Other;
                }
                break;
              case XmlState.AttributeValue:
                if (ch == '"' || ch == '\'')
                {
                  state = XmlState.Tag;
                }
                break;
              case XmlState.CData:
                if (ch == ']' && ReadText("]>"))
                {
                  state = XmlState.Other;
                }
                break;
              case XmlState.Comment:
                if (ch == '-' && ReadText("->"))
                {
                  state = XmlState.Other;
                }
                break;
              case XmlState.Tag:
                if (char.IsWhiteSpace((char)ch))
                {
                  state = XmlState.Attribute;
                }
                else if (ch == '>')
                {
                  state = XmlState.Other;
                }
                break;
              case XmlState.Other:
                if (ch == '<')
                {
                  if (_reader.Peek() == '!')
                  {
                    ReadText();
                    if (ReadText("--"))
                    {
                      state = XmlState.Comment;
                    }
                    else if (ReadText("[CDATA["))
                    {
                      state = XmlState.CData;
                    }
                  }
                  else
                  {
                    state = XmlState.Tag;
                  }
                }
                break;
            }
            break;
        }
        ch = ReadText();
      }

      switch (state)
      {
        case XmlState.Attribute:
          if (char.IsWhiteSpace(_builder[_builder.Length - 1]))
          {
            state = XmlState.AttributeStart;
            _builder.Append(">");
          }
          else
          {
            _builder.Append("=\"\">");
          }
          break;
        case XmlState.AttributeValue:
          if (_builder[_builder.Length - 1] == '=')
          {
            _builder.Append('"');
          }
          else
          {
            if (_builder[LastIndexOf('=') + 1] == '\'')
            {
              _builder.Append("'>");
            }
            else
            {
              _builder.Append("\">");
            }
          }
          break;
        case XmlState.CData:
          _builder.Append("]]>");
          break;
        case XmlState.Comment:
          _builder.Append("-->");
          break;
        case XmlState.Tag:
          if (_builder[_builder.Length - 1] == '<') _builder.Append(__noName);
          _builder.Append(">");
          break;
      }
      _builder.Append("<!--").Append(__eof).Append("-->");

      _reader = null;
      var settings = new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment };
      var textReader = new System.IO.StringReader(_builder.ToString());
      _builder.Length = 0;
      _xml = XmlReader.Create(textReader, settings);
    }

    private int LastIndexOf(char value)
    {
      var i = _builder.Length - 1;
      while (i >= 0)
      {
        if (_builder[i] == value)
          return i;
        i--;
      }
      return i;
    }
    private int ReadText()
    {
      _i++;
      var value = _reader.Read();
      if (value > 0) _builder.Append((char)value);
      return value;
    }
    private bool ReadText(char value)
    {
      if (_reader.Peek() == value)
      {
        ReadText();
        return true;
      }
      return false;
    }
    private bool ReadText(string value)
    {
      for (var i = 0; i < value.Length; i++)
      {
        if (_reader.Peek() != value[i])
          return false;
        ReadText();
      }
      return true;
    }

    public override int AttributeCount
    {
      get { return _xml.AttributeCount; }
    }

    public override string BaseURI
    {
      get { return _xml.BaseURI; }
    }

    public override int Depth
    {
      get { return _xml.Depth; }
    }

    public override bool EOF
    {
      get { return _xml.EOF || (_xml.NodeType == XmlNodeType.Comment && _xml.Value == __eof); }
    }

    public override string GetAttribute(int i)
    {
      return _xml.GetAttribute(i);
    }

    public override string GetAttribute(string name, string namespaceURI)
    {
      return _xml.GetAttribute(name, namespaceURI);
    }

    public override string GetAttribute(string name)
    {
      return _xml.GetAttribute(name);
    }

    public override bool IsEmptyElement
    {
      get { return _xml.IsEmptyElement; }
    }

    public override string LocalName
    {
      get { return _xml.LocalName; }
    }

    public override string LookupNamespace(string prefix)
    {
      return _xml.LookupNamespace(prefix);
    }

    public override bool MoveToAttribute(string name, string ns)
    {
      return _xml.MoveToAttribute(name, ns);
    }

    public override bool MoveToAttribute(string name)
    {
      return _xml.MoveToAttribute(name);
    }

    public override bool MoveToElement()
    {
      return _xml.MoveToElement();
    }

    public override bool MoveToFirstAttribute()
    {
      return _xml.MoveToFirstAttribute();
    }

    public override bool MoveToNextAttribute()
    {
      return _xml.MoveToNextAttribute();
    }

    public override XmlNameTable NameTable
    {
      get { return _xml.NameTable; }
    }

    public override string NamespaceURI
    {
      get { return _xml.NamespaceURI; }
    }

    public override XmlNodeType NodeType
    {
      get { return _xml.NodeType; }
    }

    public override string Prefix
    {
      get { return _xml.Prefix; }
    }

    public override bool Read()
    {
      return _xml.Read() && !(_xml.NodeType == XmlNodeType.Comment && _xml.Value == __eof);
    }

    public override bool ReadAttributeValue()
    {
      return _xml.ReadAttributeValue();
    }

    public override ReadState ReadState
    {
      get { return _xml.ReadState; }
    }

    public override void ResolveEntity()
    {
      _xml.ResolveEntity();
    }

    public override string Value
    {
      get { return _xml.Value; }
    }

    public bool HasLineInfo()
    {
      var lineInfo = _xml as IXmlLineInfo;
      return lineInfo != null && lineInfo.HasLineInfo();
    }

    public int LineNumber
    {
      get { return ((IXmlLineInfo)_xml).LineNumber; }
    }

    public int LinePosition
    {
      get { return ((IXmlLineInfo)_xml).LinePosition; }
    }
  }
}
