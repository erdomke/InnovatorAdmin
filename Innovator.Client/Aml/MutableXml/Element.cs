using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal abstract class Element : 
    #if NET4
    System.Dynamic.DynamicObject, 
    #endif
    IElement
  {
    protected XmlElement _node;
    protected ElementFactory _factory;

    public bool Exists
    {
      get { return _node != null; }
    }
    public virtual string Name
    {
      get { return _node.LocalName; }
    }
    internal XmlElement Node
    {
      get { return _node; }
    }
    public virtual IElement Parent
    {
      get 
      {
        return _factory.ElementFromXml((XmlElement)_node.ParentNode); 
      }
    }
    public virtual string Value
    {
      get 
      {
        if (_node == null) return null;
        return _node.InnerText; 
      }
    }

    public Element(ElementFactory factory, string name, params object[] content)
    {
      _factory = factory;
      _node = BufferDocument.CreateElement(name);
      Add((object[])content);
    }
    public Element(ElementFactory factory, XmlElement node)
    {
      _factory = factory;
      _node = node;
    }

    public virtual IElement Add(object content)
    {
      var elem = content as Element;
      if (elem != null)
      {
        _node.AppendChild(_node.GetLocalNode(elem._node));
        return this;
      }
      var attr = content as Attribute;
      if (attr != null)
      {
        _node.Attributes.SetNamedItem(_node.GetLocalNode(attr._attribute));
        return this;
      }

      var ienum = content as IEnumerable;
      if (ienum != null)
      {
        if (ienum.OfType<Element>().Any(e => e != null))
        {
          foreach (var e in ienum.OfType<Element>().Where(e => e != null))
          {
            _node.AppendChild(_node.GetLocalNode(e._node));
          }
          return this;
        }

        if (ienum.OfType<Attribute>().Any(a => a != null))
        {
          foreach (var a in ienum.OfType<Attribute>().Where(a => a != null))
          {
            _node.Attributes.SetNamedItem(_node.GetLocalNode(a._attribute));
          }
          return this;
        }
      }

      _node.InnerText = _factory.LocalizationContext.Format(content);
      return this;
    }
    public IElement Add(params object[] content)
    {
      if (content == null) return this;
      foreach (var item in content)
      {
        Add(item);
      }
      return this;
    }
    public IAttribute Attribute(string name)
    {
      if (!this.Exists)
        return Innovator.Client.Attribute.NullAttribute;

      var attr = _node.GetAttributeNode(name);
      if (attr == null) return _factory.AttributeTemplate(name, _node);
      return _factory.Attribute(attr);
    }
    public IEnumerable<IAttribute> Attributes()
    {
      return _node.Attributes.OfType<XmlAttribute>().Select(a => _factory.Attribute(a));
    }
    public IEnumerable<IElement> Elements()
    {
      return _node.ChildNodes.OfType<XmlElement>().Select(e => _factory.ElementFromXml(e));
    }
    public void Remove()
    {
      if (this.Exists) _node.ParentNode.RemoveChild(_node);
    }
    public void RemoveNodes()
    {
      _node.IsEmpty = true;
    }
    public void RemoveAttributes()
    {
      _node.RemoveAllAttributes();
    }

    public override string ToString()
    {
      if (_node == null) return null;
      return _node.OuterXml;
    }
    

    internal void SetAttribute(string name, object value)
    {
      if (value == null)
      {
        _node.Attributes.RemoveNamedItem(name);
      }
      else
      {
        _node.SetAttribute(name, _factory.LocalizationContext.Format(value));
      }
    }
    internal void SetElement(string name, object value)
    {
      var existing = _node.ChildNodes.OfType<XmlElement>().Where(e => ObjectEquals(e.LocalName, name)).ToArray();
      if (value == null)
      {
        foreach (var curr in existing)
        {
          _node.RemoveChild(curr);
        }
      }
      else
      {
        if (existing.Any())
        {
          existing[0].IsEmpty = true;
          SetElement(existing[0], value);
          foreach (var curr in existing.Skip(1))
          {
            _node.RemoveChild(curr);
          } 
        }
        else
        {
          SetElement((XmlElement)_node.AppendChild(_node.OwnerDocument.CreateElement(name)), value);
        }
      }
    }
    private void SetElement(XmlElement elem, object value)
    {
      var amlElement = value as Element;
      if (amlElement == null)
      {
        elem.InnerText = _factory.LocalizationContext.Format(value);
      }
      else
      {
        elem.AppendChild(_node.GetLocalNode(amlElement._node));
      }
    }

    internal bool TryGetAttribute(string name, out IAttribute attribute)
    {
      attribute = this.Attribute(name);
      return attribute != null && attribute.Exists;
    }
    internal bool TryGetElement(string name, out IElement element)
    {
      element = _factory.ElementFromXml(_node.ChildNodes.OfType<XmlElement>().SingleOrDefault(e => e.LocalName == name));
      return element != null;
    }

    internal static bool ObjectEquals(object x, object y)
    {
      if (x == null && y == null)
      {
        return true;
      }
      else if (x == null || y == null)
      {
        return false;
      }
      else
      {
        return x.Equals(y);
      }
    }

    //public static explicit operator Element(string value)
    //{
    //  var buffer = new XmlDocument(BufferDocument.NameTable);
    //  buffer.LoadXml(value);
    //  return FromXml(buffer.DocumentElement);
    //}

    internal static readonly XmlDocument BufferDocument = new XmlDocument();

    object ICloneable.Clone()
    {
      return _factory.ElementFromXml((XmlElement)_node.CloneNode(true));
    }

    IReadOnlyAttribute IReadOnlyElement.Attribute(string name)
    {
      return this.Attribute(name);
    }

    IEnumerable<IReadOnlyAttribute> IReadOnlyElement.Attributes()
    {
      return this.Attributes().Cast<IReadOnlyAttribute>();
    }

    IEnumerable<IReadOnlyElement> IReadOnlyElement.Elements()
    {
      return this.Elements().Cast<IReadOnlyElement>();
    }

    IReadOnlyElement IReadOnlyElement.Parent
    {
      get { return this.Parent; }
    }


    IServerContext IReadOnlyElement.Context
    {
      get { return _factory.LocalizationContext; }
    }


    public virtual string ToAml()
    {
      if (_node == null) return null;
      return _node.OuterXml;
    }

    public void ToAml(XmlWriter writer)
    {
      _node.WriteTo(writer);
    }
  }
}
