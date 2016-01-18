using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal class Property : Element, IProperty
  {
    private string _lang;
    private string _name;
    private XmlElement _parent;

    public string Language
    {
      get
      {
        return _lang ?? _node.GetAttribute("xml:lang");
      }
    }
    public override string Name
    {
      get
      {
        return _name ?? base.Name;
      }
    }
    public override IElement Parent
    {
      get
      {
        return _factory.ElementFromXml(_parent) ?? base.Parent;
      }
    }
    public override string Value
    {
      get
      {
        if (!this.Exists || _node.GetAttribute("is_null") == "1") return null;
        var item = _factory.ElementFromXml(_node.ChildNodes.OfType<XmlElement>().FirstOrDefault(e => e.LocalName == "Item")) as Item;
        if (item != null) return item.Id();
        
        var textNode = _node.ChildNodes.OfType<XmlText>().SingleOrDefault();
        if (textNode != null) return textNode.Value;
        
        var cdataNode = _node.ChildNodes.OfType<XmlCDataSection>().SingleOrDefault();
        if (cdataNode != null) return cdataNode.Value;

        return null;
      }
    }

    internal Property(ElementFactory factory, string name, params object[] content) 
      : base(factory, name, content) { }
    internal Property(ElementFactory factory, XmlElement node) 
      : base(factory, node) { }
    internal Property(ElementFactory factory, XmlElement parent, string name, string lang) 
      : base(factory, (XmlElement)null)
    {
      _name = name;
      _parent = parent;
      _lang = lang;
    }

    public override IElement Add(object content)
    {
      if (content == null)
      {
        _node.IsEmpty = true;
        SetAttribute("is_null", "1");
        return this;
      }
      else
      {
        return base.Add(content);
      }
    }

    private string NeutralValue()
    {
      if (!this.Exists || _node.GetAttribute("is_null") == "1") return null;
      if (_node.HasAttribute("neutral_value")) return _node.GetAttribute("neutral_value");

      var textNode = _node.ChildNodes.OfType<XmlText>().SingleOrDefault();
      if (textNode != null) return textNode.Value;

      var cdataNode = _node.ChildNodes.OfType<XmlCDataSection>().SingleOrDefault();
      if (cdataNode != null) return cdataNode.Value;

      return null;
    }

    public bool? AsBoolean()
    {
      if (!this.Exists) return null;
      return _factory.LocalizationContext.AsBoolean(this.NeutralValue());
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
      return _factory.LocalizationContext.AsDateTime(this.NeutralValue());
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
      return _factory.LocalizationContext.AsDateTimeUtc(this.NeutralValue());
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
      return _factory.LocalizationContext.AsInt(this.NeutralValue());
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
      return _factory.LocalizationContext.AsLong(this.NeutralValue());
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
      return _factory.LocalizationContext.AsDouble(this.NeutralValue());
    }
    public double AsDouble(double defaultValue)
    {
      var result = AsDouble();
      if (result.HasValue) return result.Value;
      return defaultValue;
    }
    public IItem AsItem()
    {
      if (!this.Exists) return Item.NullItem;
      var item = _factory.ElementFromXml(this._node.ChildNodes.OfType<XmlElement>()
                                        .FirstOrDefault(e => e.LocalName == "Item")) as IItem;
      IAttribute type;
      if (item == null && this.Value.IsGuid() && this.TryGetAttribute("type", out type))
      {
        item = _factory.Item(this._node.OwnerDocument.CreateElement("Item"));
        item.Type().Set(type.Value);
        item.Attribute("id").Set(this.Value);

        IAttribute keyedName;
        if (this.TryGetAttribute("keyed_name", out keyedName))
        {
          item.Property("keyed_name").Set(keyedName.Value);
          item.Add(_factory.Property("id", _factory.Attribute("keyed_name", keyedName.Value)
                                    , _factory.Attribute("type", type.Value), this.Value));
        }
        else
        {
          item.Add(_factory.Property("id", _factory.Attribute("type", type.Value), this.Value));
        }
      }
      return item ?? Item.NullItem;
    }
    public string AsString(string defaultValue)
    {
      if (!this.Exists || _node.GetAttribute("is_null") == "1"
        || _node.ChildNodes.OfType<XmlElement>().Any() && string.IsNullOrEmpty(this.Value)) return defaultValue;
      return this.Value;
    }
    
    public void Set(object value)
    {
      if (!Exists)
      {
        if (_parent != null && !string.IsNullOrEmpty(_name))
        {
          if (!string.IsNullOrEmpty(_lang))
          {
            if (_lang == _factory.LocalizationContext.LanguageCode)
            {
              _node = (XmlElement)_parent.AppendChild(_parent.OwnerDocument.CreateElement(_name));
            }
            else
            {
              _node = (XmlElement)_parent.AppendChild(
                _parent.OwnerDocument.CreateElement("i18n", _name, "http://www.aras.com/I18N"));
            }
            _node.SetAttribute("xml:lang", _lang);
          }
          else
          {
            _node = (XmlElement)_parent.AppendChild(_parent.OwnerDocument.CreateElement(_name));
          }
          _parent = null;
          _name = null;
          _lang = null;
        }
        else
        {
          throw new InvalidOperationException();
        }
      }

      _node.IsEmpty = true;
      if (value is DateTime) value = _factory.LocalizationContext.Format(value);
      if (value == null)
      {
        SetAttribute("is_null", "1");
      }
      else 
      {
        _node.RemoveAttribute("is_null");
        if (value is Attribute) throw new InvalidOperationException();
        _node.IsEmpty = true;
        Add(value);
      }
    }

    internal static readonly Property NullProperty = new Property(null, (XmlElement)null);


    IReadOnlyItem IReadOnlyProperty.AsItem()
    {
      return this.AsItem();
    }
  }

}
