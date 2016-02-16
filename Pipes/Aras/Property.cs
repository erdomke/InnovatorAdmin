using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Aras
{
  public class Property : IProperty
  {
    private Dictionary<string, object> _attrs = new Dictionary<string, object>();
    private string _name;
    private object _value;

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
    public string Name
    {
      get { return _name; }
      set { _name = value; }
    }
    public object Value
    {
      get { return _value; }
      set { _value = value; }
    }

    public Property() {}
    public Property(IProperty orig)
    {
      foreach (var attr in orig.Attributes)
      {
        _attrs[attr.Name] = attr.Value;
      }
      _name = orig.Name;
      _value = orig.Value;
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
        } else {
          return Data.FieldStatus.FilledIn;
        }
      }
      else
      {
        return Data.FieldStatus.Undefined;
      }
    }

    public override bool Equals(object obj)
    {
      var prop = obj as IProperty;
      if (prop == null)
      {
        return false;
      }
      else
      {
        return Equals(prop);
      }
    }
    public bool Equals(IProperty prop)
    {
      return string.Compare(_name, prop.Name, StringComparison.InvariantCultureIgnoreCase) == 0 && Extension.IsEqual(_value, prop.Value);
    }
    public override int GetHashCode()
    {
      return _name.GetHashCode() ^ _value.GetHashCode();
    }
    public override string ToString()
    {
      var builder = new StringBuilder("<");
      builder.Append(_name);
      foreach (var attr in _attrs)
      {
        builder.AppendFormat(" {0}=\"{1}\"", attr.Key, attr.Value);
      }
      if (_value == null || _value == DBNull.Value || _value == "")
      {
        builder.Append(" />");
      }
      else
      {
        builder.Append(">");
        builder.Append(_value.ToString());
        builder.Append("</");
        builder.Append(_name);
        builder.Append(">");
      }
      return builder.ToString();
    }
  }
}
