using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public enum ProcessState
  {
    DontRender = 0,
    RenderRemaining = 1,
    RenderAll = 2
  }
  public class ConfigurableXmlWriter : XmlWriter
  {
    protected XmlWriter _base;
    private bool _blockAttr = false;
    private int _blockElemDepth = 0;

    public bool AllowComments { get; set; }
    public Func<string, string, string, XmlWriter, ProcessState> AttributeProcessor { get; set; }
    public Func<string, string, string, XmlWriter, ProcessState> ElementProcessor { get; set; }


    private ConfigurableXmlWriter()
    {
      this.AllowComments = true;
    }
    public ConfigurableXmlWriter(Stream stream) : this()
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";
      _base = XmlWriter.Create(stream, settings);
    }
    public ConfigurableXmlWriter(TextWriter writer) : this()
    {
      var settings = new XmlWriterSettings();
      settings.OmitXmlDeclaration = true;
      settings.Indent = true;
      settings.IndentChars = "  ";
      _base = XmlWriter.Create(writer, settings);
    }

    public override void Close()
    {
      _base.Close();
    }

    public override void Flush()
    {
      _base.Flush();
    }

    public override string LookupPrefix(string ns)
    {
      return _base.LookupPrefix(ns);
    }

    public override void WriteBase64(byte[] buffer, int index, int count)
    {
      if (_blockElemDepth <= 0) _base.WriteBase64(buffer, index, count);
    }

    public override void WriteCData(string text)
    {
      if (_blockElemDepth <= 0) _base.WriteCData(text);
    }

    public override void WriteCharEntity(char ch)
    {
      if (_blockElemDepth <= 0) _base.WriteCharEntity(ch);
    }

    public override void WriteChars(char[] buffer, int index, int count)
    {
      if (_blockElemDepth <= 0) _base.WriteChars(buffer, index, count);
    }

    public override void WriteComment(string text)
    {
      if (_blockElemDepth <= 0 && this.AllowComments) _base.WriteComment(text);
    }

    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
      _base.WriteDocType(name, pubid, sysid, subset);
    }

    public override void WriteEndAttribute()
    {
      if (_blockElemDepth <= 0)
      {
        if (!_blockAttr) _base.WriteEndAttribute();
        _blockAttr = false;
      }
    }

    public override void WriteEndDocument()
    {
      _base.WriteEndDocument();
    }

    public override void WriteEndElement()
    {
      if (_blockElemDepth <= 0)
      {
        _base.WriteEndElement();
      }
      else
      {
        _blockElemDepth--;
      }
    }

    public override void WriteEntityRef(string name)
    {
      if (_blockElemDepth <= 0) _base.WriteEntityRef(name);
    }

    public override void WriteFullEndElement()
    {
      if (_blockElemDepth <= 0)
      {
        _base.WriteFullEndElement();
      }
      else
      {
        _blockElemDepth--;
      }
    }

    public override void WriteProcessingInstruction(string name, string text)
    {
      _base.WriteProcessingInstruction(name, text);
    }

    public override void WriteRaw(string data)
    {
      _base.WriteRaw(data);
    }

    public override void WriteRaw(char[] buffer, int index, int count)
    {
      _base.WriteRaw(buffer, index, count);
    }

    public override void WriteStartAttribute(string prefix, string localName, string ns)
    {
      if (_blockElemDepth <= 0)
      {
        var result = ProcessState.RenderAll;
        if (this.AttributeProcessor != null)
        {
          result = this.AttributeProcessor.Invoke(prefix, localName, ns, _base);
        }

        if (result == ProcessState.RenderAll) _base.WriteStartAttribute(prefix, localName, ns);
        _blockAttr = result == ProcessState.DontRender;
      }
    }

    public override void WriteStartDocument(bool standalone)
    {
      _base.WriteStartDocument(standalone);
    }

    public override void WriteStartDocument()
    {
      _base.WriteStartDocument();
    }

    public override void WriteStartElement(string prefix, string localName, string ns)
    {
      if (_blockElemDepth > 0)
      {
        _blockElemDepth++;
      }
      else
      {
        var result = ProcessState.RenderAll;
        if (this.ElementProcessor != null)
        {
          result = this.ElementProcessor.Invoke(prefix, localName, ns, _base);
        }

        if (result == ProcessState.RenderAll) _base.WriteStartElement(prefix, localName, ns);
        _blockElemDepth = (result == ProcessState.DontRender ? 1 : 0);
      }
    }

    public override WriteState WriteState
    {
      get { return _base.WriteState; }
    }

    public override void WriteString(string text)
    {
      if (!_blockAttr && _blockElemDepth <= 0) _base.WriteString(text);
    }

    public override void WriteSurrogateCharEntity(char lowChar, char highChar)
    {
      if (_blockElemDepth <= 0) _base.WriteSurrogateCharEntity(lowChar, highChar);
    }

    public override void WriteWhitespace(string ws)
    {
      if (_blockElemDepth <= 0) _base.WriteWhitespace(ws);
    }
  }
}
