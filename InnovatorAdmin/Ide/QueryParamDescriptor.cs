using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class QueryParamDescriptor : PropertyDescriptor
  {
    private QueryParameter _param;

    public QueryParameter Param { get { return _param; } }

    public QueryParamDescriptor(QueryParameter param) : base(param.Name, new Attribute[] { })
    {
      _param = param;
    }

    public override bool CanResetValue(object component)
    {
      return true;
    }

    public override Type ComponentType
    {
      get { return typeof(QueryParameter); }
    }

    public override object GetValue(object component)
    {
      return _param.GetValue();
    }

    public override bool IsReadOnly
    {
      get { return false; }
    }

    public override Type PropertyType
    {
      get
      {
        switch (_param.Type)
        {
          case QueryParameter.DataType.Boolean:
            return typeof(bool);
          case QueryParameter.DataType.DateTime:
            return typeof(DateTime);
          case QueryParameter.DataType.Decimal:
            return typeof(decimal);
          case QueryParameter.DataType.Integer:
            return typeof(long);
        }
        return typeof(string);
      }
    }

    public override void ResetValue(object component)
    {
      _param.TextValue = null;
    }

    public override void SetValue(object component, object value)
    {
      if (value is bool)
      {
        _param.TextValue = ((bool)value ? "1" : "0");
      }
      else if (value == null)
      {
        _param.TextValue = null;
      }
      else
      {
        _param.TextValue = value.ToString();
      }
    }

    public override bool ShouldSerializeValue(object component)
    {
      return false;
    }
  }
}
