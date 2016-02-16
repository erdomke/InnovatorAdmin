using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Pipes.Xml
{
  public class XmlAttributeWrapper : IXmlAttribute
  {
    private XmlAttribute _xmlAttribute;

    public string Name
    {
      get { return _xmlAttribute.Name; }
    }
    public string Value
    {
      get
      {
        return _xmlAttribute.Value;
      }
      set
      {
        _xmlAttribute.Value = value;
      }
    }  
    object Data.IFieldValue.Value
    {
      get { return _xmlAttribute.Value; }
    }
    public IXmlName XmlName
    {
      get { return new XmlNodeNameWrapper(_xmlAttribute); }
    }

    public XmlAttributeWrapper(XmlAttribute xmlAttribute)
    {
      _xmlAttribute = xmlAttribute;
    }
  }
}
