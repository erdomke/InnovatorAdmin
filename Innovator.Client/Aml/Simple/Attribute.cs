using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Aml.Simple
{
  class Attribute : IAttribute, ILink<Attribute>
  {
    private Element _parent;
    private string _name;
    private object _content;
    private Attribute _next;

    public bool Exists { get { return _next != null; } }
    public string Name { get { return _name; } }
    public Attribute Next
    {
      get { return _next; }
      set { _next = value; }
    }
    public string Value
    {
      get { return _parent.AmlContext.LocalizationContext.Format(_content); }
      set { Set(value); }
    }

    public Attribute(string name)
    {
      _name = name;
    }
    public Attribute(string name, Element parent)
    {
      _name = name;
      _parent = parent;
    }
    public Attribute(IReadOnlyAttribute attr, Element parent)
    {
      _name = attr.Name;
      _content = attr.Value;
      _parent = parent;
    }

    private static Attribute _nullAttr = new Attribute(null);
    public static Attribute NullAttr { get { return _nullAttr; } }

    public void Set(object value)
    {
      if (!Exists)
        _parent.Add(this);
      _content = value;
    }

    public void Remove()
    {
      if (Exists)
        _parent.RemoveAttribute(this);
    }

    public bool? AsBoolean()
    {
      if (!this.Exists) return null;
      return _parent.AmlContext.LocalizationContext.AsBoolean(_content);
    }
    public bool AsBoolean(bool defaultValue)
    {
      var result = AsBoolean();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public DateTime? AsDateTime()
    {
      if (!this.Exists) return null;
      return _parent.AmlContext.LocalizationContext.AsDateTime(_content);
    }
    public DateTime AsDateTime(DateTime defaultValue)
    {
      var result = AsDateTime();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public DateTime? AsDateTimeUtc()
    {
      if (!this.Exists) return null;
      return _parent.AmlContext.LocalizationContext.AsDateTimeUtc(_content);
    }
    public DateTime AsDateTimeUtc(DateTime defaultValue)
    {
      var result = AsDateTimeUtc();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public Guid? AsGuid()
    {
      if (!this.Exists || _content == null) return null;
      return new Guid(_content.ToString());
    }
    public Guid AsGuid(Guid defaultValue)
    {
      var result = AsGuid();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public int? AsInt()
    {
      if (!this.Exists) return null;
      return _parent.AmlContext.LocalizationContext.AsInt(_content);
    }
    public int AsInt(int defaultValue)
    {
      var result = AsInt();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public long? AsLong()
    {
      if (!this.Exists) return null;
      return _parent.AmlContext.LocalizationContext.AsLong(_content);
    }
    public long AsLong(long defaultValue)
    {
      var result = AsLong();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public double? AsDouble()
    {
      if (!this.Exists) return null;
      return _parent.AmlContext.LocalizationContext.AsDouble(_content);
    }
    public double AsDouble(double defaultValue)
    {
      var result = AsDouble();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public string AsString(string defaultValue)
    {
      if (!this.Exists)
        return defaultValue;
      return this.Value;
    }

    public static Attribute TryGet(object value, Element newParent)
    {
      var impl = value as Attribute;
      if (impl != null)
      {
        if (impl._parent == null || impl._parent == newParent)
          return impl;
        return new Attribute(impl, newParent);
      }

      var attr = value as IReadOnlyAttribute;
      if (attr != null)
      {
        return new Attribute(attr, newParent);
      }

      return null;
    }

  }
}
