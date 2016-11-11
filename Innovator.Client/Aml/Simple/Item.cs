using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class Item : Element, IItem
  {
    private ElementFactory _amlContext;
    private IElement _parent = AmlElement.NullElem;

    public override ElementFactory AmlContext { get { return _amlContext; } }
    /// <summary>
    /// The tag name of the AML element
    /// </summary>
    public override string Name { get { return "Item"; } }
    public override IElement Parent
    {
      get { return _parent; }
      set { _parent = value ?? AmlElement.NullElem; }
    }
    public override ILinkedElement Next
    {
      get { return ((_attr & ElementAttributes.Null) > 0 ? null : this); }
      set { /* Do nothing */ }
    }

    protected Item() { }
    public Item(ElementFactory amlContext, params object[] content)
    {
      _amlContext = amlContext;
      if (content.Length > 0)
        Add(content);
    }

    private static Dictionary<Type, IReadOnlyItem> _nullItems = new Dictionary<Type, IReadOnlyItem>()
    {
      { typeof(Item), new Item(){ _attr = ElementAttributes.ReadOnly | ElementAttributes.Null } }
    };

    public static void AddNullItem<T>(T value) where T : IReadOnlyItem
    {
      _nullItems[typeof(T)] = value;
    }
    public static T GetNullItem<T>() where T : IReadOnlyItem
    {
      IReadOnlyItem result;
      if (_nullItems.TryGetValue(typeof(T), out result))
        return (T)result;
      return default(T);
    }

    internal Item SetFlag(ElementAttributes attr)
    {
      _attr |= attr;
      return this;
    }

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

    public override IEnumerable<IElement> Elements()
    {
      if ((_attr & ElementAttributes.ItemDefaultAny) != 0)
        return GetDefaultProperties().Concat(base.Elements());
      return base.Elements();
    }
    private IEnumerable<IElement> GetDefaultProperties()
    {
      if ((_attr & ElementAttributes.ItemDefaultGeneration) != 0)
        yield return Client.Property.DefaultGeneration;
      if ((_attr & ElementAttributes.ItemDefaultIsCurrent) != 0)
        yield return Client.Property.DefaultIsCurrent;
      if ((_attr & ElementAttributes.ItemDefaultIsReleased) != 0)
        yield return Client.Property.DefaultIsReleased;
      if ((_attr & ElementAttributes.ItemDefaultMajorRev) != 0)
        yield return Client.Property.DefaultMajorRev;
      if ((_attr & ElementAttributes.ItemDefaultNewVersion) != 0)
        yield return Client.Property.DefaultNewVersion;
      if ((_attr & ElementAttributes.ItemDefaultNotLockable) != 0)
        yield return Client.Property.DefaultNotLockable;
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

          if (prop != null)
            return prop;
        }

        if (name == "generation" && (_attr & ElementAttributes.ItemDefaultGeneration) > 0)
          return Client.Property.DefaultGeneration;
        if (name == "is_current" && (_attr & ElementAttributes.ItemDefaultIsCurrent) > 0)
          return Client.Property.DefaultIsCurrent;
        if (name == "is_released" && (_attr & ElementAttributes.ItemDefaultIsReleased) > 0)
          return Client.Property.DefaultIsReleased;
        if (name == "major_rev" && (_attr & ElementAttributes.ItemDefaultMajorRev) > 0)
          return Client.Property.DefaultMajorRev;
        if (name == "new_version" && (_attr & ElementAttributes.ItemDefaultNewVersion) > 0)
          return Client.Property.DefaultNewVersion;
        if (name == "not_lockable" && (_attr & ElementAttributes.ItemDefaultNotLockable) > 0)
          return Client.Property.DefaultNotLockable;
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
      return Relationships().OfType<IReadOnlyItem>();
    }

    IEnumerable<IReadOnlyItem> IReadOnlyItem.Relationships(string type)
    {
      return Relationships(type).OfType<IReadOnlyItem>();
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
