using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  class ReadOnlyProperty : Element, IReadOnlyProperty
  {
    private string _name;
    private IElement _parent;

    public override string Name { get { return _name; } }
    public override IElement Parent
    {
      get { return _parent; }
      set { _parent = value; }
    }

    private object NeutralValue()
    {
      if (!this.Exists || Attribute("is_null").AsBoolean(false)) return null;
      var neutral = Attribute("neutral_value");
      if (neutral.HasValue()) return neutral.Value;

      return _content;
    }

    public ReadOnlyProperty(string name, params object[] content)
    {
      _name = name;
      Add(content);
    }

    public bool? AsBoolean()
    {
      if (!this.Exists) return null;
      return _parent.AmlContext.LocalizationContext.AsBoolean(NeutralValue());
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
      return _parent.AmlContext.LocalizationContext.AsDateTime(NeutralValue());
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
      return _parent.AmlContext.LocalizationContext.AsDateTimeUtc(NeutralValue());
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
      return _parent.AmlContext.LocalizationContext.AsInt(NeutralValue());
    }
    public int AsInt(int defaultValue)
    {
      var result = AsInt();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public IReadOnlyItem AsItem()
    {
      if (!this.Exists) return Item.NullItem;
      var item = _content as IItem;
      var typeAttr = Attribute("type");
      if (item == null && IsGuid() && typeAttr.Exists)
      {
        var aml = AmlContext;
        item = aml.Item(aml.Type(typeAttr.Value), aml.Id(AsGuid()));
        var keyedName = Attribute("keyed_name");
        if (keyedName.Exists)
        {
          item.Property("keyed_name").Set(keyedName.Value);
          item.Add(aml.IdProp(aml.Attribute("keyed_name", keyedName.Value), aml.Type(typeAttr.Value), AsGuid()));
        }
        else
        {
          item.Add(aml.IdProp(aml.Type(typeAttr.Value), AsGuid()));
        }
      }
      return item ?? Item.NullItem;
    }
    private bool IsGuid()
    {
      return _content is Guid || (_content is string && ((string)_content).IsGuid());
    }
    public long? AsLong()
    {
      if (!this.Exists) return null;
      return _parent.AmlContext.LocalizationContext.AsLong(NeutralValue());
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
      return _parent.AmlContext.LocalizationContext.AsDouble(NeutralValue());
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
  }
}
