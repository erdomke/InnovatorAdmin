using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal class Attribute : IAttribute
  {
    internal XmlAttribute _attribute;
    private XmlElement _parent;
    private string _name;
    private ElementFactory _factory;

    public bool Exists
    {
      get { return _attribute != null;  }
    }
    public string Name
    {
      get { return _attribute.LocalName; }
    }
    public string Value
    {
      get { return this.Exists ? _attribute.Value : null; }
      set
      {
        if (!Exists)
        {
          if (_parent != null && !string.IsNullOrEmpty(_name))
          {
            _attribute = _parent.SetAttributeNode(_name, null);
          }
          else
          {
            throw new InvalidOperationException();
          }
        }
        _attribute.Value = value;
      }
    }

    internal Attribute(ElementFactory factory, string name)
    {
      _factory = factory;
      _attribute = Element.BufferDocument.CreateAttribute(name, null);
    }
    internal Attribute(ElementFactory factory, string name, object value)
    {
      _factory = factory;
      _attribute = Element.BufferDocument.CreateAttribute(name);
      Set(value);
    }
    internal Attribute(ElementFactory factory, XmlAttribute attribute)
    {
      _factory = factory;
      _attribute = attribute;
    }
    internal Attribute(ElementFactory factory, XmlElement parent, string name)
    {
      _factory = factory;
      _parent = parent;
      _name = name;
    }

    public bool? AsBoolean()
    {
      if (!this.Exists) return null;
      return _factory.LocalizationContext.AsBoolean(this.Value);
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
      return _factory.LocalizationContext.AsDateTime(this.Value);
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
      return _factory.LocalizationContext.AsDateTimeUtc(this.Value);
    }
    public DateTime AsDateTimeUtc(DateTime defaultValue)
    {
      var result = AsDateTimeUtc();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public Guid? AsGuid()
    {
      if (!this.Exists || string.IsNullOrEmpty(this.Value)) return null;
      return new Guid(this.Value);
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
      return _factory.LocalizationContext.AsInt(this.Value);
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
      return _factory.LocalizationContext.AsLong(this.Value);
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
      return _factory.LocalizationContext.AsDouble(this.Value);
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

    public void Set(object value)
    {
      this.Value = _factory.LocalizationContext.Format(value);
    }

    internal static readonly Attribute NullAttribute = new Attribute(null, (XmlAttribute)null);

    public void Remove()
    {
      if (_attribute != null)
      {
        _parent = _attribute.OwnerElement;
        _name = _attribute.LocalName;
        _parent.RemoveAttributeNode(_attribute);
        _attribute = null;
      }
    }
  }
}
