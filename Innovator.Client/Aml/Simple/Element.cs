using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client.Aml.Simple
{
  abstract class Element : ILinkedElement
  {
    protected object _content;
    private Attribute _lastAttr;
    protected ILinkedElement _next;

    public virtual ElementFactory AmlContext
    {
      get
      {
        if (this.Parent != null)
          return this.Parent.AmlContext;
        return null;
      }
    }
    public virtual bool Exists { get { return _next != null; } }
    public abstract string Name { get; }
    public virtual ILinkedElement Next
    {
      get { return _next; }
      set
      {
        if (!string.IsNullOrEmpty(Name))
          _next = value;
      }
    }
    public abstract IElement Parent { get; }
    IReadOnlyElement IReadOnlyElement.Parent { get { return this.Parent; } }
    public string Value
    {
      get
      {
        return AmlContext.LocalizationContext.Format(_content);
      }
    }

    public Element() { }
    public Element(IReadOnlyElement elem)
    {
      Add(elem.Attributes());
      Add(elem.Elements());
      if (elem.Value != null)
        Add(elem.Value);
    }

    public virtual IElement Add(params object[] content)
    {
      if (string.IsNullOrEmpty(Name))
        throw new InvalidOperationException();

      foreach (var obj in Flatten(content))
      {
        var attr = Simple.Attribute.TryGet(obj, this);
        if (attr != null)
        {
          _lastAttr = LinkedListOps.Add(_lastAttr, attr);
        }
        else
        {
          var elem = TryGet(obj, this);
          if (elem != null)
          {
            _content = LinkedListOps.Add(_content as ILinkedElement, elem);
          }
          else
          {
            _content = obj;
          }
        }
      }
      return this;
    }

    static Element TryGet(object value, Element newParent)
    {
      var impl = value as Element;
      if (impl != null)
      {
        if (impl.Parent == null || impl.Parent == newParent)
          return impl;
        return new GenericElement(impl, newParent);
      }

      var elem = value as IReadOnlyElement;
      if (elem != null)
      {
        return new GenericElement(elem, newParent);
      }

      return null;
    }

    public static IEnumerable<object> Flatten(IEnumerable<object> content)
    {
      foreach (var value in content)
      {
        var ienum = content as IEnumerable;
        if (ienum != null
          && (ienum.OfType<IReadOnlyAttribute>().Any()
            || ienum.OfType<IReadOnlyElement>().Any()))
        {
          foreach (var obj in ienum)
          {
            yield return obj;
          }
        }
        else
        {
          yield return value;
        }
      }
    }

    public IAttribute Attribute(string name)
    {
      if (!Exists)
        return Simple.Attribute.NullAttr;
      return LinkedListOps.Find(_lastAttr, name)
        ?? new Attribute(name, this);
    }

    public IEnumerable<IAttribute> Attributes()
    {
      return LinkedListOps.Enumerate(_lastAttr);
    }

    public IEnumerable<IElement> Elements()
    {
      var lastElem = _content as ILinkedElement;
      if (lastElem == null)
        return Enumerable.Empty<IElement>();
      return LinkedListOps.Enumerate(lastElem);
    }

    public void Remove()
    {
      if (Exists)
      {
        var elem = this.Parent as Element;
        if (elem != null)
          elem.RemoveNode(this);
      }
    }
    public void RemoveAttribute(Attribute attr)
    {
      LinkedListOps.Remove(_lastAttr, attr);
    }
    public void RemoveAttributes()
    {
      _lastAttr = null;
    }
    public void RemoveNode(ILinkedElement elem)
    {
      var lastElem = _content as ILinkedElement;
      if (lastElem == null)
        return;
      LinkedListOps.Remove(lastElem, elem);
    }
    public void RemoveNodes()
    {
      _content = null;
    }

    public void ToAml(XmlWriter writer, AmlWriterSettings settings)
    {
      writer.WriteStartElement(this.Name);
      foreach (var attr in LinkedListOps.Enumerate(_lastAttr))
      {
        writer.WriteAttributeString(attr.Name, attr.Value);
      }
      var elem = _content as ILinkedElement;
      if (elem == null)
      {
        writer.WriteString(AmlContext.LocalizationContext.Format(_content));
      }
      else
      {
        var item = LinkedListOps.Enumerate(elem).OfType<IReadOnlyItem>().FirstOrDefault();
        if (this is IReadOnlyProperty
          && !settings.ExpandPropertyItems
          && item != null
          && !item.Attribute("action").Exists)
        {
          writer.WriteAttributeString("type", item.TypeName());
          var keyedName = item.KeyedName().Value ?? item.Property("id").KeyedName().Value;
          if (!string.IsNullOrEmpty(keyedName))
            writer.WriteAttributeString("keyed_name", keyedName);
          writer.WriteString(item.Id());
        }
        else
        {
          foreach (var e in LinkedListOps.Enumerate(elem))
          {
            e.ToAml(writer, settings);
          }
        }
      }
      writer.WriteEndElement();
    }

    IReadOnlyAttribute IReadOnlyElement.Attribute(string name)
    {
      return this.Attribute(name);
    }

    IEnumerable<IReadOnlyAttribute> IReadOnlyElement.Attributes()
    {
      return this.Attributes();
    }

    IEnumerable<IReadOnlyElement> IReadOnlyElement.Elements()
    {
      return this.Elements();
    }
  }
}
