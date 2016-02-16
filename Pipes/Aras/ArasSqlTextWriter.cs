using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Pipes.Data;

namespace Pipes.Aras
{
  public class ArasSqlTextWriter : Data.SqlServerTextWriter
  {
    public ArasSqlTextWriter(TextWriter writer) : base(writer) { }

    public override Code.IBaseCodeWriter DateValue(object value)
    {
      if (value == null || value is DBNull || (value is String && (string)value == ""))
      {
        return Null();
      }
      else if (value is String)
      {
        this.Write("'{0:s}'", TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse((string)value)));
        return this;
      }
      else
      {
        this.Write("'{0:s}'", TimeZoneInfo.ConvertTimeToUtc((DateTime)value));
        return this;
      }
    }
    public override Code.IBaseCodeWriter Identifier(object value)
    {
      return base.Identifier(value.ToString().Replace(" ","_"));
    }
  }
}
