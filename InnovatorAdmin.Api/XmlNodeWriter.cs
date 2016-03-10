using System;
using System.Collections.Generic;
using System.Xml;

namespace InnovatorAdmin
{
  public class XmlNodeWriter : XmlWriter
  {
    private List<XmlNode> content;
    private XmlNode parent;
    private string attrPrefix;
    private string attrNs;
    private string attrName;
    private string attrValue;
    private XmlNode root;

    public override XmlWriterSettings Settings
    {
      get
      {
        return new XmlWriterSettings
        {
          ConformanceLevel = ConformanceLevel.Auto
        };
      }
    }
    public override WriteState WriteState
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    public XmlNodeWriter(XmlNode container)
    {
      this.root = container;
    }
    public override void Close()
    {
      if (this.content == null)
        return;

      foreach (var child in this.content)
      {
        this.root.AppendChild(child);
      }
    }
    public override void Flush()
    {
    }
    public override string LookupPrefix(string namespaceName)
    {
      throw new NotSupportedException();
    }
    public override void WriteBase64(byte[] buffer, int index, int count)
    {
      throw new NotSupportedException();
    }
    public override void WriteCData(string text)
    {
      this.AddNode(GetDoc().CreateCDataSection(text));
    }
    public override void WriteCharEntity(char ch)
    {
      this.AddString(new string(ch, 1));
    }
    public override void WriteChars(char[] buffer, int index, int count)
    {
      this.AddString(new string(buffer, index, count));
    }
    public override void WriteComment(string text)
    {
      this.AddNode(GetDoc().CreateComment(text));
    }
    public override void WriteDocType(string name, string pubid, string sysid, string subset)
    {
      this.AddNode(GetDoc().CreateDocumentType(name, pubid, sysid, subset));
    }
    public override void WriteEndAttribute()
    {
      var o = GetDoc().CreateAttribute(this.attrPrefix, this.attrName, this.attrNs);
      o.Value = this.attrValue;
      this.attrName = null;
      this.attrNs = null;
      this.attrPrefix = null;
      this.attrValue = null;
      if (this.parent != null)
      {
        this.parent.Attributes.Append(o);
        return;
      }
      this.Add(o);
    }
    public override void WriteEndDocument()
    {
    }
    public override void WriteEndElement()
    {
      this.parent = ((XmlElement)this.parent).ParentNode;
    }
    public override void WriteEntityRef(string name)
    {
      if (name == "amp")
      {
        this.AddString("&");
        return;
      }
      if (name == "apos")
      {
        this.AddString("'");
        return;
      }
      if (name == "gt")
      {
        this.AddString(">");
        return;
      }
      if (name == "lt")
      {
        this.AddString("<");
        return;
      }
      if (!(name == "quot"))
      {
        throw new NotSupportedException();
      }
      this.AddString("\"");
    }
    public override void WriteFullEndElement()
    {
      var xElement = (XmlElement)this.parent;
      if (xElement.IsEmpty)
      {
        xElement.AppendChild(GetDoc().CreateTextNode(string.Empty));
      }
      this.parent = xElement.ParentNode;
    }
    public override void WriteProcessingInstruction(string name, string text)
    {
      if (name == "xml")
      {
        return;
      }
      this.AddNode(GetDoc().CreateProcessingInstruction(name, text));
    }
    public override void WriteRaw(char[] buffer, int index, int count)
    {
      this.AddString(new string(buffer, index, count));
    }
    public override void WriteRaw(string data)
    {
      this.AddString(data);
    }
    public override void WriteStartAttribute(string prefix, string localName, string namespaceName)
    {
      if (prefix == null)
      {
        throw new ArgumentNullException("prefix");
      }
      this.attrName = localName;
      this.attrNs = namespaceName;
      this.attrPrefix = prefix;
      this.attrValue = string.Empty;
    }
    public override void WriteStartDocument()
    {
    }
    public override void WriteStartDocument(bool standalone)
    {
    }
    public override void WriteStartElement(string prefix, string localName, string namespaceName)
    {
      var elem = GetDoc().CreateElement(prefix, localName, namespaceName);
      this.AddNode(elem);
    }
    public override void WriteString(string text)
    {
      this.AddString(text);
    }
    public override void WriteSurrogateCharEntity(char lowCh, char highCh)
    {
      this.AddString(new string(new char[]
      {
        highCh,
        lowCh
      }));
    }
    public override void WriteValue(DateTimeOffset value)
    {
      this.WriteString(XmlConvert.ToString(value));
    }
    public override void WriteWhitespace(string ws)
    {
      this.AddString(ws);
    }
    private void Add(XmlNode o)
    {
      if (this.content == null)
      {
        this.content = new List<XmlNode>();
      }
      this.content.Add(o);
    }
    private void AddNode(XmlNode n)
    {
      if (this.parent != null)
      {
        this.parent.AppendChild(n);
      }
      else
      {
        this.Add(n);
      }
      var elem = n as XmlElement;
      if (elem != null)
      {
        this.parent = elem;
      }
    }
    private void AddString(string s)
    {
      if (s == null)
      {
        return;
      }
      if (this.attrValue != null)
      {
        this.attrValue += s;
        return;
      }
      if (this.parent != null)
      {
        this.parent.AppendChild(GetDoc().CreateTextNode(s));
        return;
      }
      this.Add(GetDoc().CreateTextNode(s));
    }

    private XmlDocument GetDoc()
    {
      var doc = this.root as XmlDocument;
      if (doc == null)
        doc = this.root.OwnerDocument;
      return doc;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        this.Close();
      }
    }
  }
}
