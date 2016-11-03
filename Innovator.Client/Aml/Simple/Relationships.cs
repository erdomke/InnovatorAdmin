using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  class Relationships : ILinkedElement, IRelationships
  {
    private ElementAttribute _attr;
    private List<IReadOnlyItem> _relList;
    private ILinkedElement _next;
    private IElement _parent;

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
    public string Name { get { return "Relationships"; } }
    public virtual ILinkedElement Next
    {
      get { return _next; }
      set { _next = value; }
    }
    public IElement Parent
    {
      get { return _parent ?? Item.GetNullItem<Item>(); }
      set { _parent = value; }
    }
    IReadOnlyElement IReadOnlyElement.Parent { get { return this.Parent; } }
    public bool ReadOnly
    {
      get
      {
        return (_attr & ElementAttribute.ReadOnly) > 0;
      }
      set
      {
        if (value)
          _attr = _attr | ElementAttribute.ReadOnly;
        else
          _attr = _attr & ~ElementAttribute.ReadOnly;
      }
    }
    public string Value { get { return null; } }

    public Relationships() { }
    public Relationships(IElement parent)
    {
      _parent = parent;
    }
    public Relationships(params object[] content) : this()
    {
      for (var i = 0; i < content.Length; i++)
      {
        Add(content[i]);
      }
    }

    public IElement Add(object content)
    {
      var item = content as IReadOnlyItem;
      if (item == null)
      {
        var ienum = content as IEnumerable;
        if (ienum != null)
        {
          foreach (var obj in ienum)
          {
            Add(obj);
          }
        }
        return this;
      }

      if (this.ReadOnly)
        throw new InvalidOperationException();

      if (_relList == null)
        _relList = new List<IReadOnlyItem>();
      _relList.Add(item);
      var iObj = item as Item;
      if (iObj != null)
        iObj.Parent = this;
      return this;
    }

    public IAttribute Attribute(string name)
    {
      return Innovator.Client.Attribute.NullAttr;
    }

    public IEnumerable<IAttribute> Attributes()
    {
      return Enumerable.Empty<IAttribute>();
    }

    public IEnumerable<IElement> Elements()
    {
      return _relList.OfType<IElement>();
    }

    public IEnumerable<IReadOnlyItem> ByType(string type)
    {
      return _relList.OfType<IReadOnlyItem>().Where(i => i.TypeName() == type);
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
    public void RemoveAttributes()
    {
      // Do nothing
    }
    public void RemoveNodes()
    {
      _relList = null;
    }
    internal void RemoveNode(IReadOnlyItem item)
    {
      _relList.Remove(item);
      if (_relList.Count < 1)
        _relList = null;
    }
    internal void Compress()
    {
      if (_relList != null)
      {
        if (_relList.Count < 1)
          _relList = null;
        else
          _relList.TrimExcess();
      }
    }

    public void ToAml(XmlWriter writer, AmlWriterSettings settings)
    {
      writer.WriteStartElement(this.Name);
      foreach (var elem in Elements())
      {
        elem.ToAml(writer, settings);
      }
      writer.WriteEndElement();
    }

    IReadOnlyAttribute IReadOnlyElement.Attribute(string name)
    {
      return this.Attribute(name);
    }

    IEnumerable<IReadOnlyAttribute> IReadOnlyElement.Attributes()
    {
      return this.Attributes().OfType<IReadOnlyAttribute>();
    }

    IEnumerable<IReadOnlyElement> IReadOnlyElement.Elements()
    {
      return _relList.OfType<IReadOnlyElement>();
    }
  }
}
