using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client.Aml.Simple
{
  class Relationships : ILinkedElement, IRelationships
  {
    private ItemList _relList;
    private ILinkedElement _next;

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
    public IElement Parent { get; set; }
    IReadOnlyElement IReadOnlyElement.Parent { get { return this.Parent; } }
    public string Value { get { return null; } }

    public Relationships() { }

    public virtual IElement Add(params object[] content)
    {
      foreach (var item in Simple.Element.Flatten(content).OfType<IReadOnlyItem>())
      {
        var typeName = item.TypeName();
        var list = LinkedListOps.Find(_relList, typeName);
        if (list == null)
        {
          list = new ItemList() { Name = typeName };
          _relList = LinkedListOps.Add(_relList, list);
        }
        list.Add(item);
      }
      return this;
    }

    public IAttribute Attribute(string name)
    {
      return Simple.Attribute.NullAttr;
    }

    public IEnumerable<IAttribute> Attributes()
    {
      return Enumerable.Empty<IAttribute>();
    }

    public IEnumerable<IElement> Elements()
    {
      return LinkedListOps.Enumerate(_relList).SelectMany(l => l.OfType<IElement>());
    }

    public IEnumerable<IReadOnlyItem> ByType(string type)
    {
      return LinkedListOps.Find(_relList, type);
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
      return this.Attributes();
    }

    IEnumerable<IReadOnlyElement> IReadOnlyElement.Elements()
    {
      return this.Elements();
    }
  }
}
