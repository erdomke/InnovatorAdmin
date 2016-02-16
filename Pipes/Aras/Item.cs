using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Aras
{
  public class Item : IDataItem
  {
    private Dictionary<string, object> _attrs = new Dictionary<string, object>();
    private Dictionary<string, Property> _props = new Dictionary<string, Property>();

    public IEnumerable<Data.IFieldValue> Attributes
    {
      get
      {
        foreach (var kvp in _attrs)
        {
          yield return new Data.FieldValue() { Name = kvp.Key, Value = kvp.Value };
        }
      }
    }
    public int FieldCount
    {
      get { return _props.Count; }
    }
    public IEnumerable<IProperty> Properties
    {
      get { return _props.Values.Cast<IProperty>(); }
    }

    public Item() {}
    public Item(IDataItem orig) {
      foreach (var attr in orig.Attributes)
      {
        _attrs[attr.Name] = attr.Value;
      }
      foreach (var prop in orig.Properties)
      {
        _props[prop.Name] = new Property(prop);
      }
    }

    public object Attribute(string name)
    {
      object value = null;
      if (_attrs.TryGetValue(name, out value))
      {
        return value;
      }
      else
      {
        return null;
      }
    }
    public void Attribute(string name, object value)
    {
      _attrs[name] = value;
    }

    public Data.FieldStatus AttributeStatus(string name)
    {
      object value = null;
      if (_attrs.TryGetValue(name, out value))
      {
        if (value == null)
        {
          return Data.FieldStatus.Empty;
        }
        else
        {
          return Data.FieldStatus.FilledIn;
        }
      }
      else
      {
        return Data.FieldStatus.Undefined;
      }
    }

    public IProperty Property(string name)
    {
      Property prop = null;
      if (_props.TryGetValue(name, out prop))
      {
        return prop;
      }
      else
      {
        return null;
      }
    }
    public void Property(string name, IProperty value)
    {
      var prop = value as Property;
      if (prop == null) prop = new Property(value);
      _props[name] = prop;
    }
    public void Property(string name, object value)
    {
      Property prop = null;
      if (!_props.TryGetValue(name, out prop))
      {
        prop = new Property() { Name = name };
        _props[name] = prop;
      }
      prop.Value = value;
    }

    object Data.IDataRecord.Item(string name)
    {
      return Property(name).Value;
    }

    public Data.FieldStatus Status(string name)
    {
      Property prop = null;
      if (_props.TryGetValue(name, out prop))
      {
        if ((string)prop.Attribute("is_null") == "1")
        {
          return Data.FieldStatus.Null;
        }
        else if (prop.Value == null || (prop.Value is string && (string)prop.Value == string.Empty))
        {
          return Data.FieldStatus.Empty;
        }
        else
        {
          return Data.FieldStatus.FilledIn;
        }
      }
      else
      {
        return Data.FieldStatus.Undefined;
      }
    }

    public IEnumerator<Data.IFieldValue> GetEnumerator()
    {
      return _props.Values.Cast<Data.IFieldValue>().GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public override string ToString()
    {
      var builder = new StringBuilder("<Item");
      foreach (var attr in _attrs)
      {
        builder.AppendFormat(" {0}=\"{1}\"", attr.Key, attr.Value);
      }
      builder.AppendLine(">");
      foreach (var prop in _props.Values)
      {
        builder.Append("  ");
        builder.Append(prop.ToString());
        builder.AppendLine();
      }
      builder.Append("</Item>");
      return base.ToString();
    }
  }
}
