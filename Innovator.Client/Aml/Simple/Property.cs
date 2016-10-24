using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  class Property : Element, IProperty
  {
    private string _name;
    private ILinkedElement _next;
    private IElement _parent;

    public override string Name { get { return _name; } }
    public override ILinkedElement Next
    {
      get { return _next; }
      set { _next = value; }
    }
    public override IElement Parent
    {
      get { return _parent ?? AmlElement.NullElem; }
      set { _parent = value; }
    }

    private object NeutralValue()
    {
      if (!this.Exists || Attribute("is_null").AsBoolean(false)) return null;
      var neutral = Attribute("neutral_value");
      if (neutral.HasValue()) return neutral.Value;

      return _content;
    }

    public Property(string name, params object[] content)
    {
      _name = name;
      if (content == null)
        this.IsNull().Set(true);
      else if (content.Length > 0)
        Add(content);
    }
    public Property(IElement parent, string name)
    {
      _name = name;
      _parent = parent;
    }

    private static Property _nullProp;
    private static Property _defaultGeneration;
    private static Property _defaultIsCurrent;
    private static Property _defaultIsReleased;
    private static Property _defaultMajorRev;
    private static Property _defaultNewVersion;
    private static Property _defaultNotLockable;

    static Property()
    {
      _nullProp = new Property(null) { ReadOnly = true };
      _defaultGeneration = new Property("generation", "1") { ReadOnly = true };
      _defaultGeneration.Next = _defaultGeneration;
      _defaultIsCurrent = new Property("is_current", "1") { ReadOnly = true };
      _defaultIsCurrent.Next = _defaultIsCurrent;
      _defaultIsReleased = new Property("is_released", "0") { ReadOnly = true };
      _defaultIsReleased.Next = _defaultIsReleased;
      _defaultMajorRev = new Property("major_rev", "A") { ReadOnly = true };
      _defaultMajorRev.Next = _defaultMajorRev;
      _defaultNewVersion = new Property("new_version", "0") { ReadOnly = true };
      _defaultNewVersion.Next = _defaultNewVersion;
      _defaultNotLockable = new Property("not_lockable", "0") { ReadOnly = true };
      _defaultNotLockable.Next = _defaultNotLockable;
    }

    public static Property NullProp { get { return _nullProp; } }
    public static Property DefaultGeneration { get { return _defaultGeneration; } }
    public static Property DefaultIsCurrent { get { return _defaultIsCurrent; } }
    public static Property DefaultIsReleased { get { return _defaultIsReleased; } }
    public static Property DefaultMajorRev { get { return _defaultMajorRev; } }
    public static Property DefaultNewVersion { get { return _defaultNewVersion; } }
    public static Property DefaultNotLockable { get { return _defaultNotLockable; } }

    public bool? AsBoolean()
    {
      if (!this.Exists) return null;
      return (_parent == null ? ElementFactory.Local : _parent.AmlContext).LocalizationContext.AsBoolean(NeutralValue());
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
      return (_parent == null ? ElementFactory.Local : _parent.AmlContext).LocalizationContext.AsDateTime(NeutralValue());
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
      return (_parent == null ? ElementFactory.Local : _parent.AmlContext).LocalizationContext.AsDateTimeUtc(NeutralValue());
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
      return (_parent == null ? ElementFactory.Local : _parent.AmlContext).LocalizationContext.AsInt(NeutralValue());
    }
    public int AsInt(int defaultValue)
    {
      var result = AsInt();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public IItem AsItem()
    {
      if (!this.Exists) return Item.NullItem;
      var item = _content as IItem;
      var typeAttr = Attribute("type");
      if (item == null && IsGuid() && typeAttr.Exists)
      {
        var aml = AmlContext ?? ElementFactory.Local;
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
      return (_parent == null ? ElementFactory.Local : _parent.AmlContext).LocalizationContext.AsLong(NeutralValue());
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
      return (_parent == null ? ElementFactory.Local : _parent.AmlContext).LocalizationContext.AsDouble(NeutralValue());
    }
    public double AsDouble(double defaultValue)
    {
      var result = AsDouble();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public string AsString(string defaultValue)
    {
      if (!this.Exists || Attribute("is_null").AsBoolean(false))
        return defaultValue;
      return this.Value;
    }

    public override IElement Add(object content)
    {
      var result = base.Add(content);
      var isNull = this.IsNull();
      if (_content == null || _content == DBNull.Value)
      {
        isNull.Set(true);
      }
      else
      {
        isNull.Remove();
      }
      return result;
    }

    public void Set(object value)
    {
      AssertModifiable();
      if (!Exists)
      {
        if (_parent == null)
          throw new InvalidOperationException();
        _parent.Add(this);
      }

      var isNull = this.IsNull();
      if (value == null || value == DBNull.Value)
      {
        isNull.Set(true);
      }
      else
      {
        isNull.Remove();
      }
      _content = value;
    }

    IReadOnlyItem IReadOnlyProperty_Item.AsItem()
    {
      return AsItem();
    }
  }
}
