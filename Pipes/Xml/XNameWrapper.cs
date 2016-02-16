using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Pipes.Xml
{
  public class XNameWrapper : IXmlName
  {
    private XName _name;
    private XElement _parent;

    public string LocalName
    {
      get { return _name.LocalName; }
    }

    public string Namespace
    {
      get { return _name.NamespaceName; }
    }

    public string Prefix
    {
      get { return _parent.GetPrefixOfNamespace(_name.Namespace); }
    }

    public XNameWrapper(XName name, XElement parent)
    {
      _name = name;
      _parent = parent;
    }

    public override string ToString()
    {
      if (_name.Namespace == null || _name.Namespace == _parent.GetDefaultNamespace())
      {
        return this.LocalName;
      }
      else
      {
        return this.Prefix + ":" + this.LocalName;
      }
    }
  }
}
