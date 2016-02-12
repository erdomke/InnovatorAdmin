using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  public abstract class ItemWrapper : IReadOnlyItem
  {
    private IReadOnlyItem _item;

    public ItemWrapper(IReadOnlyItem item)
    {
      _item = item;
    }

    public IReadOnlyResult AsResult()
    {
      return _item.AsResult();
    }

    public IItem Clone()
    {
      return _item.Clone();
    }

    public string Id()
    {
      return _item.Id();
    }

    public virtual IReadOnlyProperty Property(string name)
    {
      return _item.Property(name);
    }

    public virtual IReadOnlyProperty Property(string name, string lang)
    {
      return _item.Property(name, lang);
    }

    public IEnumerable<IReadOnlyItem> Relationships()
    {
      return _item.Relationships();
    }

    public IEnumerable<IReadOnlyItem> Relationships(string type)
    {
      return _item.Relationships(type);
    }

    public IReadOnlyAttribute Attribute(string name)
    {
      return _item.Attribute(name);
    }

    public IEnumerable<IReadOnlyAttribute> Attributes()
    {
      return _item.Attributes();
    }

    public IEnumerable<IReadOnlyElement> Elements()
    {
      return _item.Elements();
    }

    public IServerContext Context
    {
      get { return _item.Context; }
    }

    public bool Exists
    {
      get { return _item.Exists; }
    }

    public string Name
    {
      get { return _item.Name; }
    }

    public IReadOnlyElement Parent
    {
      get { return _item.Parent; }
    }

    public string Value
    {
      get { return _item.Value; }
    }

    public string ToAml()
    {
      return _item.ToAml();
    }

    public void ToAml(XmlWriter writer)
    {
      _item.ToAml(writer);
    }

    object ICloneable.Clone()
    {
      return _item.Clone();
    }
  }
}
