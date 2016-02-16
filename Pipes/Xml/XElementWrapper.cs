using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Pipes.Xml
{
  public class XElementWrapper : IXmlElement
  {
    private XElement _xElement = null;

    public object Attribute(string name)
    {
      if (_xElement.Attribute(name) == null)
      {
        return null;
      }
      else
      {
        return _xElement.Attribute(name).Value;
      }
    }
    public void Attribute(string name, string value)
    {
      _xElement.SetAttributeValue(name, value);
    }
    public System.Collections.Generic.IEnumerable<Data.IFieldValue> Attributes
    {
      get
      {
        return (from e in _xElement.Attributes() select (Data.IFieldValue)new XAttributeWrapper(e));
      }
    }
    public bool HasElements
    {
      get { return _xElement.HasElements; }
    }
    public IXmlName Name
    {
      get { return new XNameWrapper(_xElement.Name, _xElement); }
    }
    public IXmlElement Parent
    {
      get { return new XElementWrapper(_xElement.Parent); }
    }
    public string Value
    {
      get 
      {
        var textNode = _xElement.Nodes().OfType<XText>().SingleOrDefault();
        if (textNode != null) return textNode.Value;

        var cdataNode = _xElement.Nodes().OfType<XCData>().SingleOrDefault();
        if (cdataNode != null) return cdataNode.Value;

        return null;
      }
      set { _xElement.Value = value; }
    }
    public XElement WrappedElement
    {
      get { return _xElement; }
    }


    public XElementWrapper(XElement xElement)
    {
      _xElement = xElement;
    }
    public IXmlElement Element(int index)
    {
      var elems = Elements();
      if (elems == null)
      {
        return null;
      }
      else
      {
        return elems.ElementAt(index);
      }
    }
    public IXmlElement Element(string name)
    {
      var elem = _xElement.Element(name);
      if (elem == null)
      {
        return null;
      }
      else
      {
        return new XElementWrapper(elem);
      }
    }
    public System.Collections.Generic.IEnumerable<IXmlElement> Elements()
    {
      return (from e in _xElement.Elements() select (IXmlElement)new XElementWrapper(e));
    }
    public System.Collections.Generic.IEnumerable<IXmlElement> Elements(string name)
    {
      return (from e in _xElement.Elements(name) select (IXmlElement)new XElementWrapper(e));
    }
    public IEnumerable<IXmlNode> Nodes()
    {
      return (from n in _xElement.Nodes() select Util.GetXmlNode(n));
    }
    public IXmlElement XPathSelectElement(string xpath)
    {
      var result = _xElement.XPathSelectElement(xpath);
      if (result == null)
        return null;
      return new XElementWrapper(result);
    }
    public System.Collections.Generic.IEnumerable<IXmlElement> XPathSelectElements(string xpath)
    {
      var result = _xElement.XPathSelectElements(xpath);
      if (result == null)
        return null;
      return (from n in result select (IXmlElement)new XElementWrapper(n));
    }

    public override string ToString()
    {
      return _xElement.ToString();
    }


    public XmlNodeType Type
    {
      get
      {
        if (_xElement.HasAttributes || _xElement.HasElements || !string.IsNullOrEmpty(_xElement.Value))
        {
          return XmlNodeType.Element;
        }
        else
        {
          return XmlNodeType.EmptyElement;
        }
      }
    }

    object IXmlNode.Value
    {
      get { return this.Value; }
    }
    object IXmlElement.Value
    {
      get { return this.Value; }
      set { this.Value = value.NullToString(); }
    }

    int Data.IDataRecord.FieldCount
    {
      get { return _xElement.Attributes().Count(); }
    }

    object Data.IDataRecord.Item(string name)
    {
      return Attribute(name);
    }

    Data.FieldStatus Data.IDataRecord.Status(string name)
    {
      return AttributeStatus(name);
    }
    IEnumerator<Data.IFieldValue> IEnumerable<Data.IFieldValue>.GetEnumerator()
    {
      return (from e in _xElement.Attributes() select (Data.IFieldValue) new XAttributeWrapper(e)).GetEnumerator();
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<Data.IFieldValue>)this).GetEnumerator();
    }

    public Data.FieldStatus AttributeStatus(string name)
    {
      if (_xElement.Attribute(name) == null)
      {
        return Data.FieldStatus.Undefined;
      }
      else
      {
        if (string.IsNullOrEmpty(_xElement.Attribute(name).Value))
        {
          return Data.FieldStatus.Empty;
        }
        else
        {
          return Data.FieldStatus.FilledIn;
        }
      }
    }

    IEnumerator<IXmlFieldValue> IEnumerable<IXmlFieldValue>.GetEnumerator()
    {
      return (from e in _xElement.Attributes() select (IXmlFieldValue)new XAttributeWrapper(e)).GetEnumerator();      
    }
  }
}
