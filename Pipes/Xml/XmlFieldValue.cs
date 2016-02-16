using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public class XmlFieldValue : IXmlFieldValue
  {
    public IXmlName XmlName { get; set; }
    public string Name 
    {
      get
      {
        return this.XmlName.ToString();
      }
    }
    public object Value { get; set; }

    public XmlFieldValue() { }
    public XmlFieldValue(string name, object value) : this (new XmlName(name), value) { }
    public XmlFieldValue(IXmlName name, object value)
    {
      this.XmlName = name;
      this.Value = value;
    }
    public XmlFieldValue(IXmlFieldValue item) : this(item.XmlName, item.Value) { }

    public override string ToString()
    {
      return this.Name.ToString() + "=\"" + (this.Value ?? "").ToString() + "\"";
    }
  }
}
