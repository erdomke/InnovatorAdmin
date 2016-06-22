using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Aml.Simple
{
  class Item : Element, IItem
  {
    public override string Name { get { return "Item"; } }
    public override IElement Parent { get { return null; } }
    public override ILinkedElement Next
    {
      get { return this; }
      set { /* Do nothing */ }
    }

    private Item(bool exists)
    {
      if (exists)
        _next = this;
    }

    private static Item _nullItem = new Item(false);
    public static Item NullItem { get { return _nullItem; } }

    public override IElement Add(params object[] content)
    {
      if (this == _nullItem)
        throw new InvalidOperationException();
      return base.Add(content);
    }

    public IProperty Property(string name)
    {
      if (Exists)
      {
        var elem = _content as ILinkedElement;
        if (elem != null)
        {
          return (LinkedListOps.Find(elem, name) as IProperty)
            ?? new Property(name, this);
        }
        return new Property(name, this);
      }
      return Simple.Property.NullProp;
    }

    public IProperty Property(string name, string lang)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<IItem> Relationships()
    {
      if (Exists)
      {
        var elem = _content as ILinkedElement;
        if (elem != null)
        {
          var rel = LinkedListOps.Find(elem, "Relationships") as Relationships;
          if (rel != null)
          {
            return rel.Elements().OfType<IItem>();
          }
        }
      }
      return Enumerable.Empty<IItem>();
    }

    public IEnumerable<IItem> Relationships(string type)
    {
      if (Exists)
      {
        var elem = _content as ILinkedElement;
        if (elem != null)
        {
          var rel = LinkedListOps.Find(elem, "Relationships") as Relationships;
          if (rel != null)
          {
            return rel.ByType(type).OfType<IItem>();
          }
        }
      }
      return Enumerable.Empty<IItem>();
    }

    public IItem Clone()
    {
      throw new NotImplementedException();
    }

    IReadOnlyProperty IReadOnlyItem.Property(string name)
    {
      return Property(name);
    }

    IReadOnlyProperty IReadOnlyItem.Property(string name, string lang)
    {
      return Property(name, lang);
    }

    IEnumerable<IReadOnlyItem> IReadOnlyItem.Relationships()
    {
      return Relationships();
    }

    IEnumerable<IReadOnlyItem> IReadOnlyItem.Relationships(string type)
    {
      return Relationships(type);
    }

    public string Id()
    {
      return Attribute("id").Value;
    }

    public virtual string TypeName()
    {
      return Attribute("type").Value;
    }
  }
}
