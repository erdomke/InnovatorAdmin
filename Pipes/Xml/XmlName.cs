using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Xml
{
  public class XmlName : IXmlName
  {
    public string LocalName { get; set; }
    public string Namespace { get; set; }
    public string Prefix { get; set; }

    public XmlName()
    {
    }
    public XmlName(string nameString)
    {
      var parts = nameString.Split(':');
      switch (parts.Length)
      {
        case 1:
          this.LocalName = parts[0];
          break;
        case 2:
          this.Prefix = parts[0];
          this.LocalName = parts[1];
          break;
        default:
          throw new ArgumentException("Can't have more than one ':' in an XML name");
      }
    }

    public override bool Equals(object obj)
    {
      var name = obj as IXmlName;
      if (name == null)
      {
        return false;
      }
      else
      {
        return Equals(name);
      }
    }
    public bool Equals(IXmlName obj)
    {
      return StringEquivalent(obj.LocalName, this.LocalName) && StringEquivalent(obj.Namespace, this.Namespace) && StringEquivalent(obj.Prefix, this.Prefix);
    }
    private bool StringEquivalent(string x, string y)
    {
      return (string.IsNullOrEmpty(x) && string.IsNullOrEmpty(y)) || x == y;
    }

    public override int GetHashCode()
    {
      return this.LocalName.GetHashCode() ^ this.Namespace.GetHashCode() ^ this.Prefix.GetHashCode();
    }
    public override string ToString()
    {
      if (string.IsNullOrEmpty(this.Prefix))
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
