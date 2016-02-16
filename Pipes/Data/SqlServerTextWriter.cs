using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pipes.Data
{
  public class SqlServerTextWriter : Pipes.Code.BaseCodeTextWriter
  {
    public SqlServerTextWriter(TextWriter writer) : base(writer) { }

    public override Code.IBaseCodeWriter DateValue(object value)
    {
      if (value == null || value is DBNull || (value is String && (string)value == ""))
      {
        return Null();
      }
      else if (value is String)
      {
        this.Write("'{0:s}'", DateTime.Parse((string)value));
        return this;
      }
      else
      {
        this.Write("'{0:s}'", (DateTime)value);
        return this;
      }
    }

  }
}
