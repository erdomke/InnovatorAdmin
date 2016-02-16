using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  public class FieldValue : IFieldValue
  {
    private string _name;
    private object _value;

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

    public FieldValue() { }
    public FieldValue(string name, object value) 
    {
      _name = name;
      _value = value;
    }
    public FieldValue(IFieldValue orig)
    {
      _name = orig.Name;
      _value = orig.Value;
    }

    public override bool Equals(object obj)
    {
      var fieldVal = obj as IFieldValue;
      if (fieldVal == null)
      {
        return false;
      }
      else
      {
        return Equals(fieldVal);
      }
    }
    public bool Equals(IFieldValue fieldVal)
    {
      return _name == fieldVal.Name && Extension.IsEqual(_value, fieldVal.Value);
    }
    public override int GetHashCode()
    {
      return _name.GetHashCode() ^ _value.GetHashCode();
    }
  }
}
