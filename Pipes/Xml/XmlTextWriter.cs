using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Pipes.Xml
{
  public class XmlTextWriter : TextWriter, Sgml.ISgmlTextWriter
  {
    // Conformance level
    private bool _writeValue = false;
    private System.Xml.XmlWriter _writer;

    public object Parent { get; set; }

    public static XmlTextWriter Create(TextWriter writer)
    {
      return new XmlTextWriter(writer);
    }
    public static XmlTextWriter Create(TextWriter writer, XmlWriterSettings settings)
    {
      return new XmlTextWriter(writer, settings);
    }

    public XmlTextWriter() { }

    public T Initialize<T>(T coreWriter) where T : TextWriter
    {
      _writer = System.Xml.XmlWriter.Create(coreWriter);
      return coreWriter;
    }

    protected XmlTextWriter(TextWriter writer)
    {
      Initialize(writer);
    }
    protected XmlTextWriter(TextWriter writer, XmlWriterSettings settings)
    {
      _writer = System.Xml.XmlWriter.Create(writer, settings.GetInnerSettings());
    }

    public Sgml.ISgmlWriter Attribute(string name, object value)
    {
      if (!name.StartsWith("xmlns:")) _writer.WriteAttributeString(name, (value == null ? "" : value.ToString()));
      return this;
    }
    public Sgml.ISgmlWriter Attribute(string name, string ns, object value)
    {
      _writer.WriteAttributeString(name, ns, (value == null ? "" : value.ToString()));
      
      return this;
    }
    public Sgml.ISgmlWriter Attribute(string prefix, string name, string ns, object value)
    {
      if (prefix != "xmlns") _writer.WriteAttributeString(prefix, name, ns, (value == null ? "" : value.ToString()));
      return this;
    }

    public Sgml.ISgmlWriter Comment(string value)
    {
      _writer.WriteComment(value);
      return this;
    }

    public Sgml.ISgmlWriter Element(string name, object value)
    {
      _writer.WriteElementString(name, (value == null ? "" : value.ToString()));
      return this;
    }

    public Sgml.ISgmlWriter Element(string name)
    {
      _writer.WriteStartElement(name);
      return this;
    }

    public Sgml.ISgmlWriter ElementEnd()
    {
      _writer.WriteEndElement();
      return this;
    }

    public override void Flush()
    {
      _writer.Flush();
    }
    protected override void Dispose(bool disposing)
    {
      Flush();
      base.Dispose(disposing);
    }

    public Sgml.ISgmlWriter Raw(string value)
    {
      _writeValue = false;
      Write(value);
      return this;
    }

    public Sgml.ISgmlWriter Value(object value)
    {
      try
      {
        _writeValue = true;
        Write((value == null ? "" : value.ToString()));
      }
      finally
      {
        _writeValue = false;
      }
      return this;
    }

    public override Encoding Encoding
    {
      get { return _writer.Settings.Encoding; }
    }

    public override void Write(char value)
    {
      Write(value.ToString());
    }
    public override void Write(char[] buffer, int index, int count)
    {
      if (_writeValue)
      {
        _writer.WriteValue(new string(buffer, index, count));
      }
      else
      {
        _writer.WriteRaw(buffer, index, count);
      }
    }
    public override void Write(string value)
    {
      if (_writeValue)
      {
        _writer.WriteValue(value); 
      }
      else
      {
        _writer.WriteRaw(value);
      }
    }

    public TextWriter Raw()
    {
      _writeValue = false;
      return this;
    }

    public Sgml.ISgmlWriter RawEnd()
    {
      _writeValue = false;
      return this;
    }

    public TextWriter Value()
    {
      _writeValue = true;
      return this;
    }
    public Sgml.ISgmlWriter Whitespace(string value)
    {
      _writer.WriteWhitespace(value);
      return this;
    }

    public Sgml.ISgmlWriter ValueEnd()
    {
      _writeValue = false;
      return this;
    }


    public Sgml.ISgmlWriter NsElement(string name, string ns)
    {
      _writer.WriteStartElement(name, ns);
      return this;
    }

    public Sgml.ISgmlWriter NsElement(string prefix, string name, string ns)
    {
      _writer.WriteStartElement(prefix, name, ns);
      return this;
    }
  }
}
