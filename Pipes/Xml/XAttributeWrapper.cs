using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Pipes.Xml
{
  public class XAttributeWrapper : IXmlAttribute
  {
    private XAttribute _xAttribute;
    public XAttributeWrapper(XAttribute xAttribute)
    {
      _xAttribute = xAttribute;
    }
    
    public string Name
    {
      get 
      {
        if (_xAttribute.Name.Namespace == null || _xAttribute.Name.Namespace == _xAttribute.Parent.GetDefaultNamespace())
        {
          return _xAttribute.Name.LocalName;
        }
        else
        {
          return _xAttribute.Parent.GetPrefixOfNamespace(_xAttribute.Name.Namespace) + ":" + _xAttribute.Name.LocalName;
        }
      }
    }
    public string Value
    {
      get { return _xAttribute.Value; }
      set { _xAttribute.Value = value; }
    }
    public IXmlName XmlName
    {
      get { return new XNameWrapper(_xAttribute.Name, _xAttribute.Parent); }
    }


    object Data.IFieldValue.Value
    {
      get { return _xAttribute.Value; }
    }

  }
}
