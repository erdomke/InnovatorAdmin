using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Pipes.Xml
{
  public class XmlElementWrapper : IXmlElement, IQueryableNode
  {
    private XmlElement _xmlElement = null;
    
    public bool HasElements
    {
      get { return _xmlElement.HasChildNodes; }
    }
    public IXmlName Name
    {
      get { return new XmlNodeNameWrapper(_xmlElement); }
    }
    public IXmlElement Parent
    {
      get {
        var elem = _xmlElement.ParentNode as System.Xml.XmlElement;
        if (elem == null) return null;
        return new XmlElementWrapper(elem); 
      }
    }
    public string Value
    {
      get
      {
        var textNode = _xmlElement.ChildNodes.OfType<XmlText>().SingleOrDefault();
        if (textNode != null) return textNode.Value;

        var cdataNode = _xmlElement.ChildNodes.OfType<XmlCDataSection>().SingleOrDefault();
        if (cdataNode != null) return cdataNode.Value;

        return null;
      }
      set { _xmlElement.InnerText = value; }
    }

    public XmlElementWrapper(XmlElement xmlElement)
    {
      _xmlElement = xmlElement;
    }

    public System.Collections.Generic.IEnumerable<Data.IFieldValue> Attributes
    {
      get {
        return (from System.Xml.XmlNode e in _xmlElement.Attributes select (Data.IFieldValue)new XmlAttributeWrapper((XmlAttribute)e));
      }
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
      foreach (System.Xml.XmlNode childNode in _xmlElement.ChildNodes)
      {
        if (childNode is XmlElement && childNode.Name == name)
        {
          return new XmlElementWrapper((XmlElement)childNode);
        }
      }
      return null;
    }
    public System.Collections.Generic.IEnumerable<IXmlElement> Elements()
    {
      return (from System.Xml.XmlNode e in _xmlElement.ChildNodes where e is XmlElement select (IXmlElement)new XmlElementWrapper((XmlElement)e));
    }
    public System.Collections.Generic.IEnumerable<IXmlElement> Elements(string name)
    {
      return (from System.Xml.XmlNode e in _xmlElement.ChildNodes where e is XmlElement && ((XmlElement)e).Name == name select (IXmlElement)new XmlElementWrapper((XmlElement)e));
    }
    public IEnumerable<IXmlNode> Nodes()
    {
      return (from n in _xmlElement.ChildNodes.Cast<System.Xml.XmlNode>() select Util.GetXmlNode(n));
    }
    public IXmlElement XPathSelectElement(string xpath)
    {
      var result = _xmlElement.SelectSingleNode(xpath) as System.Xml.XmlElement;
      if (result == null) return null;
      return new XmlElementWrapper(result);
    }
    public System.Collections.Generic.IEnumerable<IXmlElement> XPathSelectElements(string xpath)
    {
      var result = _xmlElement.SelectNodes(xpath);
      if (result == null) return null;
      return (from System.Xml.XmlNode n in result where n is XmlElement select (IXmlElement)new XmlElementWrapper((XmlElement)n));
    }

    public override string ToString()
    {
      return _xmlElement.ToString();
    }

    public object Attribute(string name)
    {
      if (_xmlElement.Attributes[name] == null)
      {
        return null;
      }
      else
      {
        return _xmlElement.Attributes[name].Value;
      }
    }

    public void Attribute(string name, string value)
    {
      if (_xmlElement.Attributes[name] == null)
      {
        var attr = _xmlElement.OwnerDocument.CreateAttribute(name);
        attr.Value = value;
        _xmlElement.Attributes.Append(attr);
      }
      else
      {
        _xmlElement.Attributes[name].Value = value;
      }
    }


    public XmlNodeType Type
    {
      get
      {
        if (_xmlElement.HasAttributes || _xmlElement.HasChildNodes || !string.IsNullOrEmpty(_xmlElement.Value))
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
      get { return _xmlElement.Attributes.Count; }
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
      return (from System.Xml.XmlNode e in _xmlElement.Attributes select (Data.IFieldValue)new XmlAttributeWrapper((XmlAttribute)e)).GetEnumerator();
    }
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<Data.IFieldValue>)this).GetEnumerator();
    }


    public Data.FieldStatus AttributeStatus(string name)
    {
      if (_xmlElement.Attributes[name] == null)
      {
        return Data.FieldStatus.Undefined;
      }
      else
      {
        if (string.IsNullOrEmpty(_xmlElement.Attributes[name].Value))
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
      return (from System.Xml.XmlNode e in _xmlElement.Attributes select (IXmlFieldValue)new XmlAttributeWrapper((XmlAttribute)e)).GetEnumerator();
    }

    IXmlName IQueryableNode.Name
    {
      get { return this.Name; }
    }

    bool IQueryableNode.TryGetAttributeValue(IXmlName name, StringComparison comparison, out string value)
    {
      value = null;
      var attr = _xmlElement.Attributes.Cast<XmlAttribute>()
                  .FirstOrDefault(a => string.Compare(a.LocalName, name.LocalName, comparison) == 0 &&
                                       ((string.IsNullOrEmpty(a.NamespaceURI) && string.IsNullOrEmpty(name.Namespace)) ||
                                       string.Compare(a.NamespaceURI, name.Namespace, comparison) == 0));
      if (attr == null)
      {
        return false;
      }
      else
      {
        value = attr.Value;
        return true;
      }
    }

    bool IQueryableNode.IsEmpty()
    {
      return !_xmlElement.HasChildNodes;
    }

    IQueryableNode IQueryableNode.Parent()
    {
      var parent = _xmlElement.ParentNode as XmlElement;
      if (parent == null) return null;
      return new XmlElementWrapper(parent);
    }

    IQueryableNode IQueryableNode.PrecedingSibling()
    {
      var prev = _xmlElement.PreviousSibling;
      while (prev != null)
      {
        if (prev is XmlElement) return new XmlElementWrapper(prev as XmlElement);
        prev = prev.PreviousSibling;
      }
      return null;
    }

    IQueryableNode IQueryableNode.FollowingSibling()
    {
      var next = _xmlElement.NextSibling;
      while (next != null)
      {
        if (next is XmlElement) return new XmlElementWrapper(next as XmlElement);
        next = next.NextSibling;
      }
      return null;
    }
  }
}
