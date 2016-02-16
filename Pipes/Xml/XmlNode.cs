using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes;

namespace Pipes.Xml
{
  public class XmlNode : IXmlNode
  {
    private List<IXmlFieldValue> _attr;

    public IXmlName Name { get; set; }
    public XmlNodeType Type { get; set; }
    public object Value { get; set; }

    public XmlNode() 
    {
      _attr = new List<IXmlFieldValue>();
    }
    public XmlNode(IXmlNode clone) 
    {
      this.Name = clone.Name;
      this.Type = clone.Type;
      this.Value = clone.Value;
      _attr = new List<IXmlFieldValue>(from a in (IEnumerable<IXmlFieldValue>)clone select (IXmlFieldValue)new XmlFieldValue(a));
    }
    public XmlNode(string name) : this()
    {
      this.Name = new XmlName(name);
      _attr = new List<IXmlFieldValue>();
    }
    public XmlNode(string name, IEnumerable<IXmlFieldValue> attributes) : this(new XmlName(name), attributes) { }
    public XmlNode(IXmlName name, IEnumerable<IXmlFieldValue> attributes) : this()
    {
      this.Name = name;
      this.Type = XmlNodeType.Element;
      _attr = attributes.ToList();
    }

    public int FieldCount
    {
      get { return _attr.Count; }
    }
    public object Item(string name)
    {
      int index = IndexOf(name);
      if (index < 0) throw new KeyNotFoundException();
      return _attr[index].Value;
    }
    public void Item(IXmlName name, object value)
    {
      int index = IndexOf(name);
      if (index < 0)
      {
        _attr.Add(new XmlFieldValue(name, value));
      }
      else
      {
        ((XmlFieldValue)_attr[index]).Value = value;
      }
    }
    public void Item(IXmlFieldValue value)
    {
      Item(value.XmlName, value.Value);
    }
    private int IndexOf(IXmlName name)
    {
      for (int i = 0; i < _attr.Count; i++)
      {
        if (_attr[i].XmlName.Equals(name)) return i;
      }
      return -1;
    }
    private int IndexOf(string name)
    {
      for (int i = 0; i < _attr.Count; i++)
      {
        if (_attr[i].Name == name) return i;
      }
      return -1;
    }
    public Data.FieldStatus Status(string name)
    {
      int index = IndexOf(name);
      if (index < 0)
      {
        return Data.FieldStatus.Undefined;
      }
      else if (_attr[index].Value == null)
      {
        return Data.FieldStatus.Null;
      }
      else if (_attr[index].Value.ToString() == "")
      {
        return Data.FieldStatus.Empty;
      }
      else
      {
        return Data.FieldStatus.FilledIn;
      }
    }

    IEnumerator<Data.IFieldValue> IEnumerable<Data.IFieldValue>.GetEnumerator()
    {
      return _attr.Cast<Data.IFieldValue>().GetEnumerator();
    }
    public IEnumerator<IXmlFieldValue> GetEnumerator()
    {
      return _attr.GetEnumerator();
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public override string ToString()
    {
      switch (this.Type)
      {
        case XmlNodeType.Attribute:
          return this.Name.ToString() + "=\"" + (this.Value ?? "").ToString() + "\"";
        case XmlNodeType.CDATA:
          return "[CDATA] " + (this.Value ?? "").ToString();
        case XmlNodeType.Comment:
          return "<!-- " + (this.Value ?? "").ToString() + " -->";
        case XmlNodeType.DocumentType:
          return "<!DOCTYPE " + (this.Value ?? "").ToString();
        case XmlNodeType.Element:
          return "<" + this.Name.ToString() + " " + _attr.GroupConcat(" ") + ">";
        case XmlNodeType.EmptyElement:
          return "<" + this.Name.ToString() + " " + _attr.GroupConcat(" ") + " />";
        case XmlNodeType.EndElement:
          return "</" + this.Name.ToString() + ">";
        case XmlNodeType.Text:
        case XmlNodeType.Whitespace:
          return (this.Value ?? "").ToString();
        default:
          return "{Name: " + this.Name.ToString() + ", Type: " + this.Type.ToString() + ", Value: " + (this.Value ?? "").ToString();
      }
    }
  }
}
