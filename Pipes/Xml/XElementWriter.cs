using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Pipes.Xml
{
  public class XElementWriter : Sgml.ISgmlWriter
  {
    private XElement _currNode;

    public XElementWriter(XElement elem)
    {
      _currNode = elem;
    }

    public Sgml.ISgmlWriter Attribute(string name, object value)
    {
      _currNode.SetAttributeValue(name, value);
      return this;
    }

    public Sgml.ISgmlWriter Comment(string value)
    {
      _currNode.Add(new XComment(value));
      return this;
    }

    public Sgml.ISgmlWriter Element(string name, object value)
    {
      _currNode.Add(new XElement(name, value));
      return this;
    }

    public Sgml.ISgmlWriter Element(string name)
    {
      var newNode = new XElement(name);
      _currNode.Add(newNode);
      _currNode = newNode;
      return this;
    }

    public Sgml.ISgmlWriter ElementEnd()
    {
      _currNode = _currNode.Parent;
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
        _currNode.Add(new XText(value));
      }
      else
      {
        _currNode.Add(XElement.Parse(value));
      }
      return this;
    }

    public Sgml.ISgmlWriter Value(object value)
    {
      _currNode.Value = (value == null ? "" : value.ToString());
      return this;
    }


    public Sgml.ISgmlWriter NsElement(string name, string ns)
    {
      var newNode = new XElement(XName.Get(name, ns));
      _currNode.Add(newNode);
      _currNode = newNode;
      return this;
    }
    public Sgml.ISgmlWriter NsElement(string prefix, string name, string ns)
    {
      XElement newNode; 
      if (string.IsNullOrEmpty(prefix))
      {
        newNode = new XElement(XName.Get(name, ns));
      }
      else
      {
        newNode = new XElement(XName.Get(name, ns), new XAttribute(XNamespace.Xmlns + prefix, ns));
      }
      _currNode.Add(newNode);
      _currNode = newNode;
      return this;
    }
    public Sgml.ISgmlWriter Attribute(string name, string ns, object value)
    {
      throw new NotImplementedException();
    }
    public Sgml.ISgmlWriter Attribute(string prefix, string name, string ns, object value)
    {
      _currNode.SetAttributeValue(XName.Get(name, ns), value);
      return this;
    }
  }
}
