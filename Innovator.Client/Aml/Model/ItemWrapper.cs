using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  /// <summary>
  /// Wraps an Aras item so that additional functionality can be provided
  /// </summary>
  public abstract class ItemWrapper : IReadOnlyItem
  {
    private IReadOnlyItem _item;

    public ItemWrapper(IReadOnlyItem item)
    {
      _item = item;
    }

    /// <inheritdoc/>
    public IReadOnlyResult AsResult()
    {
      return _item.AsResult();
    }
    /// <inheritdoc/>
    public IItem Clone()
    {
      return _item.Clone();
    }
    /// <inheritdoc/>
    public string Id()
    {
      return _item.Id();
    }
    /// <inheritdoc/>
    public virtual IReadOnlyProperty Property(string name)
    {
      return _item.Property(name);
    }
    /// <inheritdoc/>
    public virtual IReadOnlyProperty Property(string name, string lang)
    {
      return _item.Property(name, lang);
    }
    /// <inheritdoc/>
    public IEnumerable<IReadOnlyItem> Relationships()
    {
      return _item.Relationships();
    }
    /// <inheritdoc/>
    public IEnumerable<IReadOnlyItem> Relationships(string type)
    {
      return _item.Relationships(type);
    }
    /// <inheritdoc/>
    public IReadOnlyAttribute Attribute(string name)
    {
      return _item.Attribute(name);
    }
    /// <inheritdoc/>
    public IEnumerable<IReadOnlyAttribute> Attributes()
    {
      return _item.Attributes();
    }
    /// <inheritdoc/>
    public IEnumerable<IReadOnlyElement> Elements()
    {
      return _item.Elements();
    }
    /// <inheritdoc/>
    public IServerContext Context
    {
      get { return _item.Context; }
    }
    /// <inheritdoc/>
    public bool Exists
    {
      get { return _item.Exists; }
    }
    /// <inheritdoc/>
    public string Name
    {
      get { return _item.Name; }
    }
    /// <inheritdoc/>
    public IReadOnlyElement Parent
    {
      get { return _item.Parent; }
    }
    /// <inheritdoc/>
    public string Value
    {
      get { return _item.Value; }
    }
    /// <inheritdoc/>
    public string ToAml()
    {
      return _item.ToAml();
    }
    /// <inheritdoc/>
    public void ToAml(XmlWriter writer)
    {
      _item.ToAml(writer);
    }

    object ICloneable.Clone()
    {
      return _item.Clone();
    }
    /// <inheritdoc/>
    public string TypeName()
    {
      return _item.TypeName();
    }
  }
}
