using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Pipes.Xml
{
  public class XmlNodeNameWrapper : IXmlName
  {
    private System.Xml.XmlNode _xmlElement = null;

    public string LocalName
    {
      get { return _xmlElement.LocalName; }
    }

    public string Namespace
    {
      get { return _xmlElement.NamespaceURI; }
    }

    public string Prefix
    {
      get { return _xmlElement.Prefix; }
    }

    public override string ToString()
    {
      return _xmlElement.Name;
    }

    public XmlNodeNameWrapper(System.Xml.XmlNode xmlElement)
    {
      _xmlElement = xmlElement;
    }
  }
}
