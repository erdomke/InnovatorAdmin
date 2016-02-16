using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Pipes.Xml
{
  public class XmlElementWriter : Sgml.ISgmlWriter, IWriter<XmlElement>, IXmlDocumentWriter
  {
    private XmlElement _currNode;
    private XmlDocument _currDoc;

    public XmlElementWriter() { }
    public XmlElementWriter(XmlElement elem)
    {
      _currNode = elem;
      _currDoc = elem.OwnerDocument;
    }

    public Sgml.ISgmlWriter Attribute(string name, object value)
    {
      _currNode.SetAttribute(name, (value ?? "").ToString());
      return this;
    }

    public Sgml.ISgmlWriter Comment(string value)
    {
      _currNode.AppendChild(_currDoc.CreateComment(value));
      return this;
    }

    public Sgml.ISgmlWriter Element(string name, object value)
    {
      var newNode = _currDoc.CreateElement(name);
      if (value != null) newNode.InnerText = value.ToString();
      if (_currNode == null)
      {
        _currDoc.AppendChild(newNode);
      }
      else
      {
        _currNode.AppendChild(newNode);
      }
      return this;
    }

    public Sgml.ISgmlWriter Element(string name)
    {
      var newNode = _currDoc.CreateElement(name);
      ((System.Xml.XmlNode)_currNode ?? _currDoc).AppendChild(newNode);
      _currNode = newNode;
      return this;
    }

    public Sgml.ISgmlWriter ElementEnd()
    {
      _currNode = _currNode.ParentNode as XmlElement;
      return this;
    }

    public void Flush()
    {
      //Do Nothing...edits are already flushed
    }

    public Sgml.ISgmlWriter Raw(string value)
    {
      if (value == null)
      {
        // Do nothing
      }
      else if (value.Length > 0 && value.Trim().Length < 1)
      {
        if (_currNode != null) _currNode.AppendChild(_currDoc.CreateWhitespace(value));
      }
      else if (value.StartsWith("<!"))
      {
        // Ignore doctypes for now
      }
      else
      {
        var newDoc = new XmlDocument();
        newDoc.LoadXml(value);
        _currNode.AppendChild(_currDoc.ImportNode(newDoc.DocumentElement, true));
      }
      return this;
    }

    public Sgml.ISgmlWriter Value(object value)
    {
      _currNode.AppendChild(_currDoc.CreateTextNode(value == null ? "" : value.ToString()));
      return this;
    }


    public Sgml.ISgmlWriter NsElement(string name, string ns)
    {
      var newNode = _currDoc.CreateElement(name, ns);
      ((System.Xml.XmlNode)_currNode ?? _currDoc).AppendChild(newNode);
      _currNode = newNode;
      return this;
    }
    public Sgml.ISgmlWriter NsElement(string prefix, string name, string ns)
    {
      XmlElement newNode; 
      if (string.IsNullOrEmpty(prefix))
      {
        newNode = _currDoc.CreateElement(name, ns);
      }
      else
      {
        newNode = _currDoc.CreateElement(prefix, name, ns);
      }
      ((System.Xml.XmlNode)_currNode ?? _currDoc).AppendChild(newNode);
      _currNode = newNode;
      return this;
    }
    public Sgml.ISgmlWriter Attribute(string name, string ns, object value)
    {
      _currNode.SetAttribute(name, ns, (value ?? "").ToString());
      return this;
    }
    public Sgml.ISgmlWriter Attribute(string prefix, string name, string ns, object value)
    {
      var attr = _currDoc.CreateAttribute(prefix, name, ns);
      _currNode.SetAttributeNode(attr).Value = (value ?? "").ToString();
      return this;
    }

    public T Initialize<T>(T coreWriter) where T : XmlElement
    {
      _currNode = coreWriter;
      _currDoc = coreWriter.OwnerDocument;
      return coreWriter;
    }

    public object Parent { get; set; }

    T IWriter<XmlDocument>.Initialize<T>(T coreWriter)
    {
      _currDoc = coreWriter;
      return coreWriter;
    }
  }
}
