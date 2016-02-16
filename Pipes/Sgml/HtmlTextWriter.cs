using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Web.UI;

namespace Pipes.Sgml
{
  public class HtmlTextWriter : TextWriter, ISgmlTextWriter
  {
    private bool _currTagClosed = true;
    private int _line = 1;
    private Stack<TagInfo> _openTags = new Stack<TagInfo>();
    private bool _outputStarted = false;
    private Dictionary<string, string> _nsDict = new Dictionary<string, string>();
    private int _numSpaces = 0;
    private HtmlWriterSettings _settings;
    private System.Web.UI.HtmlTextWriter _writer;
    private bool _writeEncoded = false;

    public HtmlTextWriter() { }

    public object Parent { get; set; }

    public T Initialize<T>(T coreWriter) where T : TextWriter
    {
      _writer = new System.Web.UI.HtmlTextWriter(coreWriter);
      _settings = new HtmlWriterSettings();
      return coreWriter;
    }

    public static HtmlTextWriter Create(TextWriter writer)
    {
      return new HtmlTextWriter()
      {
        _writer = new System.Web.UI.HtmlTextWriter(writer),
        _settings = new HtmlWriterSettings()
      };
    }
    public static HtmlTextWriter Create(TextWriter writer, HtmlWriterSettings settings)
    {
      return new HtmlTextWriter()
      {
        _writer = new System.Web.UI.HtmlTextWriter(writer),
        _settings = settings
      };
    }

    public ISgmlWriter Attribute(string name, object value)
    {
      if (_settings.NewLineOnAttributes) RenderIndent(_openTags.Count + 1);
      _writer.WriteAttribute(name, (value ?? "").ToString().Replace("\"", "&quot;"));
      return this;
    }
    public ISgmlWriter Comment(string value)
    {
      CloseCurrElement();
      RenderIndent();
      _writer.Write("<!--" + value + "-->");
      return this;
    }
    public ISgmlWriter Element(string name, object value)
    {
      CloseCurrElement();
      RenderIndent();
      _writer.WriteBeginTag(name);
      if (value == null || value is DBNull || (value is string && (string)value == "")) {
        _writer.Write(System.Web.UI.HtmlTextWriter.SelfClosingChars);
        _writer.Write(System.Web.UI.HtmlTextWriter.TagRightChar);
      } else {
        _writer.Write(System.Web.UI.HtmlTextWriter.TagRightChar);
        try {
          _writeEncoded = true;
          Write(value.ToString());
        } finally {
          _writeEncoded = false;
        }
        _writer.WriteEndTag(name);
      }
      return this;
    }
    public ISgmlWriter Element(string name)
    {
      CloseCurrElement();
      RenderIndent();
      _writer.WriteBeginTag(name);
      _openTags.Push(new TagInfo(name, _line));
      _currTagClosed = false;
      return this;
    }
    public ISgmlWriter ElementEnd()
    {
      if (_currTagClosed)
      {
        var tag = _openTags.Pop();
        if (tag.Line != _line) RenderIndent();
        _writer.WriteEndTag(tag.Name);
      }
      else
      {
        _writer.Write(System.Web.UI.HtmlTextWriter.SelfClosingChars);
        _writer.Write(System.Web.UI.HtmlTextWriter.TagRightChar);
        _openTags.Pop();
      }
      _currTagClosed = true;
      return this;
    }
    public override void Flush()
    {
      _writer.Flush();
    }
    public ISgmlWriter Raw(string value)
    {
      CloseCurrElement();
      _writer.Write(value);
      return this;
    }
    public ISgmlWriter Value(object value)
    {
      if (value != null && !(value is string && ((string)value) == "") && !(value is DBNull))
      {
        CloseCurrElement();
        try
        {
          _writeEncoded = true;
          Write(value);
        }
        finally
        {
          _writeEncoded = false;
        }
      }
      return this;
    }
    public TextWriter Raw()
    {
      CloseCurrElement();
      _writeEncoded = false;
      return this;
    }
    public ISgmlWriter RawEnd()
    {
      _writeEncoded = false;
      return this;
    }
    public TextWriter Value()
    {
      CloseCurrElement();
      _writeEncoded = true;
      return this;
    }
    public ISgmlWriter ValueEnd()
    {
      _writeEncoded = false;
      return this;
    }

    private void CloseCurrElement()
    {
      if (!_currTagClosed) _writer.Write(System.Web.UI.HtmlTextWriter.TagRightChar);
      _currTagClosed = true;
    }
    private void RenderIndent()
    {
      RenderIndent(_openTags.Count);
    }
    private void RenderIndent(int indent)
    {
      if (_settings.Indent)
      {
        if (_outputStarted)
        {
          _writer.Write(_settings.NewLineChars);
          _line += 1;
        }
        for (var i = 0; i < indent; i++)
        {
          _writer.Write(_settings.IndentChars);
        }
      }
      _outputStarted = true;
    }

    private class TagInfo
    {
      public string Name { get; set; }
      public int Line { get; set; }

      public TagInfo(string name, int line)
      {
        this.Name = name;
        this.Line = line;
      }
    }

    public override Encoding Encoding
    {
      get { return _writer.Encoding; }
    }
    public override void Write(char value)
    {
      Write(value.ToString());
    }
    public override void Write(char[] buffer, int index, int count)
    {
      Write(new string(buffer, index, count));
    }
    public override void Write(string value)
    {
      var encoded = _writeEncoded && !(_openTags.Count > 0 && _openTags.Peek().Name.ToLowerInvariant() == "script");

      if (encoded)
      {
        if (_settings.ReplaceConsecutiveSpaceNonBreaking)
        {
          var startFlush = 0;
          for (var i = 0; i < value.Length; i++)
          {
            if ((int)value[i] == 160)
            {
              _numSpaces = 0;
              if (startFlush < i) HttpUtility.HtmlEncode(value.Substring(startFlush, i - startFlush), _writer);
              _writer.Write("&nbsp;");
              startFlush = i + 1;
            }
            else if (Char.GetUnicodeCategory(value[i]) == System.Globalization.UnicodeCategory.SpaceSeparator)
            {
              _numSpaces++;
              if (_numSpaces > 1)
              {
                if (startFlush < i) HttpUtility.HtmlEncode(value.Substring(startFlush, i - startFlush), _writer);
                _writer.Write("&nbsp;");
                startFlush = i + 1;
              }
            }
            else
            {
              _numSpaces = 0;
            }
          }
          if (startFlush < value.Length) HttpUtility.HtmlEncode(value.Substring(startFlush, value.Length - startFlush), _writer);
        }
        else
        {
          _writer.WriteEncodedText(value);
        }
      }
      else
      {
        _writer.Write(value);
      }
    }


    public ISgmlWriter NsElement(string name, string ns)
    {
      if (string.IsNullOrEmpty(ns))
      {
        return Element(name);
      }
      else
      {
        string prefix = null;
        if (_nsDict.TryGetValue(ns, out prefix))
        {
          return Element(prefix + ":" + name);
        }
        else
        {
          throw new ArgumentException("Undeclared namespace");
        }
      }
    }
    public ISgmlWriter NsElement(string prefix, string name, string ns)
    {
      if (string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(ns))
      {
        return Element(name);
      }
      else
      {
        _nsDict[ns] = prefix;
        return Element(prefix + ":" + name);
      }
    }
    public Sgml.ISgmlWriter Attribute(string name, string ns, object value)
    {
      if (string.IsNullOrEmpty(ns))
      {
        return Attribute(name, value);
      }
      else
      {
        string prefix = null;
        if (_nsDict.TryGetValue(ns, out prefix))
        {
          return Attribute(prefix + ":" + name, value);
        }
        else
        {
          throw new ArgumentException("Undeclared namespace");
        }
      }
    }
    public Sgml.ISgmlWriter Attribute(string prefix, string name, string ns, object value)
    {
      if (string.IsNullOrEmpty(prefix) || value == null || value == string.Empty)
      {
        return Attribute(name, value);
      }
      else
      {
        if (prefix == "xmlns")
        {
          _nsDict[value.ToString()] = name;
        }
        else
        {
          _nsDict[ns] = prefix;
        }
        return Attribute(prefix + ":" + name, value);
      }
    }
  }
}
