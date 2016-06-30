using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class Item : Element, IItem
  {
    private ElementFactory _amlContext;


    public override ElementFactory AmlContext { get { return _amlContext; } }
    public override string Name { get { return "Item"; } }
    public override IElement Parent
    {
      get { return AmlElement.NullElem; }
      set { /* Do nothing */ }
    }
    public override ILinkedElement Next
    {
      get { return this; }
      set { /* Do nothing */ }
    }

    private Item()
    {
      _next = this;
    }
    public Item(ElementFactory amlContext, params object[] content) : this()
    {
      _amlContext = amlContext;
      if (content.Length > 0)
        Add(content);
    }

    private static Item _nullItem = new Item() { _next = null, ReadOnly = true };
    public static Item NullItem { get { return _nullItem; } }

    public IProperty Property(string name)
    {
      return (((IReadOnlyItem)this).Property(name) as IProperty)
        ?? Client.Property.NullProp;
    }

    public IProperty Property(string name, string lang)
    {
      return (((IReadOnlyItem)this).Property(name, lang) as IProperty)
        ?? Client.Property.NullProp;
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
      var writer = new ResultWriter(this.AmlContext, null, null);
      ToAml(writer, new AmlWriterSettings());
      return writer.Result.AssertItem();
    }


    IReadOnlyProperty IReadOnlyItem.Property(string name)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(name);

      if (Exists)
      {
        var elem = _content as ILinkedElement;
        if (elem != null)
        {
          var prop = LinkedListOps.Find(elem, name) as IReadOnlyProperty;
          // The first one should be in the user's language if multiple languages
          // exist
          if (prop != null && prop.Attribute("xml:lang").Exists)
          {
            prop = LinkedListOps.FindAll(elem, name)
              .OfType<IReadOnlyProperty>()
              .FirstOrDefault(p => p.Attribute("xml:lang").Value == AmlContext.LocalizationContext.LanguageCode);

          }

          return prop ?? new Property(this, name);
        }
        return new Property(this, name);
      }
      return Innovator.Client.Property.NullProp;
    }

    IReadOnlyProperty IReadOnlyItem.Property(string name, string lang)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(name);
      if (string.IsNullOrEmpty(lang))
        return ((IReadOnlyItem)this).Property(name);

      if (Exists)
      {
        var elem = _content as ILinkedElement;
        if (elem != null)
        {
          var prop = LinkedListOps.FindAll(elem, name)
            .OfType<IReadOnlyProperty>()
            .FirstOrDefault(p => !p.Attribute("xml:lang").Exists
              || p.Attribute("xml:lang").Value == lang);

          if (prop != null)
            return prop;
        }
        var result = new Property(this, name);
        result.Add(new Attribute("xml:lang", lang));
        return result;
      }
      return Innovator.Client.Property.NullProp;
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
      return ((IReadOnlyElement)this).Attribute("id").Value;
    }

    public virtual string TypeName()
    {
      return ((IReadOnlyElement)this).Attribute("type").Value;
    }
  }
}
