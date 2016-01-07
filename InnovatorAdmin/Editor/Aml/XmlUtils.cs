using ICSharpCode.AvalonEdit.Document;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace InnovatorAdmin.Editor
{
  internal static class XmlUtils
  {
    public static string Attribute(this XmlElement elem, string localName, string defaultValue = null)
    {
      if (elem != null && elem.HasAttribute(localName))
      {
        return elem.Attributes[localName].Value;
      }
      else
      {
        return defaultValue;
      }
    }
    public static XmlElement Element(this XmlNode node, string localName)
    {
      return node.ChildNodes.OfType<XmlElement>().SingleOrDefault(e => e.LocalName == localName);
    }
    public static string Element(this XmlNode node, string localName, string defaultValue)
    {
      var elem = node.ChildNodes.OfType<XmlElement>().SingleOrDefault(e => e.LocalName == localName);
      if (elem == null) return defaultValue;
      return elem.InnerText;
    }
    public static IEnumerable<XmlElement> ElementsByXPath(this XmlNode node, string xPath)
    {
      return node.SelectNodes(xPath).OfType<XmlElement>();
    }

    //[DebuggerStepThrough()]
    public static XmlState ProcessFragment(ITextSource fragment, Func<XmlReader, int, XmlState, bool> processor)
    {
      var lineOffsets = new List<int>() {0};
      var state = XmlState.Other;
      var line = 1;
      var lastTag = new KeyValuePair<int, int>(0, 0);
      var attrValueQuote = '"';

      for (var i = 0; i < fragment.TextLength; i++)
      {
        switch (fragment.GetCharAt(i))
        {
          case '\r':
            if (i + 1 < fragment.TextLength && fragment.GetCharAt(i + 1) == '\n') i++;
            line++;
            lineOffsets.Add(i + 1);
            if (state == XmlState.Tag) state = XmlState.Attribute;
            break;
          case '\n':
            line++;
            lineOffsets.Add(i + 1);
            if (state == XmlState.Tag) state = XmlState.Attribute;
            break;
          default:
            switch (state)
            {
              case XmlState.Attribute:
                if (fragment.GetCharAt(i) == '=')
                {
                  i++;
                  if (i < fragment.TextLength)
                    attrValueQuote = fragment.GetCharAt(i);
                  state = XmlState.AttributeValue;
                }
                else if (fragment.GetCharAt(i) == '>')
                {
                  state = XmlState.Other;
                }
                break;
              case XmlState.AttributeValue:
                if (fragment.GetCharAt(i) == '"' || fragment.GetCharAt(i) == '\'')
                {
                  state = XmlState.Tag;
                }
                break;
              case XmlState.CData:
                if (i + 2 < fragment.TextLength && fragment.GetCharAt(i) == ']' && fragment.GetCharAt(i + 1) == ']' && fragment.GetCharAt(i + 2) == '>')
                {
                  i += 2;
                  state = XmlState.Other;
                }
                break;
              case XmlState.Comment:
                if (i + 2 < fragment.TextLength && fragment.GetCharAt(i) == '-' && fragment.GetCharAt(i + 1) == '-' && fragment.GetCharAt(i + 2) == '>')
                {
                  i += 2;
                  state = XmlState.Other;
                }
                break;
              case XmlState.Tag:
                if (char.IsWhiteSpace(fragment.GetCharAt(i)))
                {
                  state = XmlState.Attribute;
                }
                else if (fragment.GetCharAt(i) == '>')
                {
                  state = XmlState.Other;
                }
                break;
              case XmlState.Other:
                if (fragment.GetCharAt(i) == '<')
                {
                  if (i + 3 < fragment.TextLength
                    && fragment.GetCharAt(i + 1) == '!'
                    && fragment.GetCharAt(i + 2) == '-'
                    && fragment.GetCharAt(i + 3) == '-')
                  {
                    i += 3;
                    state = XmlState.Comment;
                  }
                  if (i + 8 < fragment.TextLength
                    && fragment.GetCharAt(i + 1) == '!'
                    && fragment.GetCharAt(i + 2) == '['
                    && fragment.GetCharAt(i + 3) == 'C'
                    && fragment.GetCharAt(i + 4) == 'D'
                    && fragment.GetCharAt(i + 5) == 'A'
                    && fragment.GetCharAt(i + 6) == 'T'
                    && fragment.GetCharAt(i + 7) == 'A'
                    && fragment.GetCharAt(i + 8) == '[')
                  {
                    i += 8;
                    state = XmlState.CData;
                  }
                  else
                  {
                    state = XmlState.Tag;
                    lastTag = new KeyValuePair<int, int>(line, i - lineOffsets.Last() + 2);
                  }
                }
                break;
            }
            break;
        }
      }

      const string __noName = "___NO_NAME___";
      const string __eof = "{`EOF`}";
      var suffix = string.Empty;

      switch (state)
      {
        case XmlState.Attribute:
          if (char.IsWhiteSpace(fragment.GetCharAt(fragment.TextLength - 1)))
          {
            state = XmlState.AttributeStart;
            suffix += ">";
          }
          else
          {
            suffix += "=\"\">";
          }
          break;
        case XmlState.AttributeValue:
          if (fragment.GetCharAt(fragment.TextLength - 1) == '=')
          {
            suffix += "''>";
          }
          else
          {
            suffix += attrValueQuote.ToString() + ">";
          }
          break;
        case XmlState.CData:
          suffix += "]]>";
          break;
        case XmlState.Comment:
          suffix += "-->";
          break;
        case XmlState.Tag:
          if (fragment.GetCharAt(fragment.TextLength - 1) == '<') suffix += __noName;
          suffix += ">";
          break;
      }
      suffix += "<!--" + __eof + "-->";

      var settings = new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Fragment };
      var textReader = new AugmentedReader(fragment.CreateReader(), suffix);
      var reader = XmlReader.Create(textReader, settings);
      var lineInfo = reader as IXmlLineInfo;

      try
      {
        bool keepGoing = true;
        while (keepGoing && reader.Read() && !(reader.NodeType == XmlNodeType.Comment && reader.Value == __eof))
        {
          if (reader.LocalName != __noName)
          {
            keepGoing = processor.Invoke(reader, lineOffsets[lineInfo.LineNumber - 1]
                                                + lineInfo.LinePosition
                                                - (reader.NodeType == XmlNodeType.Element ? 2 : 1), state);
            if (reader.NodeType == XmlNodeType.Element
              && lineInfo.LineNumber == lastTag.Key
              && lineInfo.LinePosition == lastTag.Value)
            {
              for (var i = 0; i < reader.AttributeCount; i++)
              {
                reader.MoveToAttribute(i);
                keepGoing = processor.Invoke(reader, lineOffsets[lineInfo.LineNumber - 1] + lineInfo.LinePosition - 1, state);
              }
            }
          }
        }
      }
      catch (XmlException)
      {
        // Do Nothing
      }

      return state;
    }

    private class AugmentedReader : TextReader
    {
      private string _suffix;
      private int _i = 0;
      private TextReader _reader;
      private bool _readerAtEnd;

      public AugmentedReader(TextReader reader, string suffix)
      {
        _reader = reader;
        _suffix = suffix;
      }

      public override int Peek()
      {
        if (!_readerAtEnd)
        {
          var result = _reader.Peek();
          if (result >= 0)
            return result;
        }

        if (_i >= _suffix.Length)
          return -1;
        return _suffix[_i];
      }
      public override int Read()
      {
        if (!_readerAtEnd)
        {
          var result = _reader.Read();
          if (result >= 0)
            return result;
          _readerAtEnd = true;
        }

        if (_i >= _suffix.Length)
          return -1;
        _i++;
        return _suffix[_i - 1];
      }
      public override int Read(char[] buffer, int index, int count)
      {
        if (!_readerAtEnd)
        {
          var result = _reader.Read(buffer, index, count);
          if (result > 0)
            return result;
          _readerAtEnd = true;
        }

        var toCopy = Math.Min(count, _suffix.Length - _i);
        if (toCopy <= 0)
          return 0;
        _suffix.CopyTo(_i, buffer, index, toCopy);
        _i += toCopy;
        return toCopy;
      }

      protected override void Dispose(bool disposing)
      {
        base.Dispose(disposing);
        if (disposing)
          _reader.Dispose();
      }
    }
  }
}
