using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin
{
  public class QueryParameter
  {
    public enum DataType
    {
      String,
      Boolean,
      DateTime,
      Decimal,
      Integer,
      Null
    }

    public string Name { get; set; }
    public string TextValue { get; set; }
    public DataType Type { get; set; }

    public object GetValue()
    {
      switch (this.Type)
      {
        case DataType.Boolean:
          if (this.TextValue == "1" || (this.TextValue ?? "").Equals("true", StringComparison.OrdinalIgnoreCase))
            return true;
          if (this.TextValue == "0" || (this.TextValue ?? "").Equals("false", StringComparison.OrdinalIgnoreCase))
            return false;
          throw new FormatException(string.Format("`{0}` is not a valid boolean value", TextValue));
        case DataType.DateTime:
          return DateTime.Parse(TextValue);
        case DataType.Decimal:
          return decimal.Parse(TextValue);
        case DataType.Integer:
          return long.Parse(TextValue);
        case DataType.Null:
          if (string.IsNullOrEmpty(this.TextValue))
            return null;
          throw new FormatException("The value must be empty if the type is Null");
        default:
          return TextValue;
      }
    }

    public void Overwrite(QueryParameter value)
    {
      this.Name = value.Name;
      this.TextValue = value.TextValue;
      this.Type = value.Type;
    }
    public QueryParameter Clone()
    {
      return new QueryParameter()
      {
        Name = this.Name,
        TextValue = this.TextValue,
        Type = this.Type
      };
    }
  }
}
