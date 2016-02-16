using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pipes.Data
{
  public class OracleSqlTextWriter : Pipes.Code.BaseCodeTextWriter
  {
    public OracleSqlTextWriter(TextWriter writer) : base(writer) { }

    public override Code.IBaseCodeWriter DateValue(object value)
    {
      if (value == null || value is DBNull || (value is String && (string)value == ""))
      {
        return Null();
      }
      else if (value is String)
      {
        this.Write("to_date('{0:yyyy-MM-dd HH:mm:ss}', 'YYYY-MM-DD HH:MI:SS')", DateTime.Parse((string)value));
        return this;
      }
      else
      {
        this.Write("to_date('{0:yyyy-MM-dd HH:mm:ss}', 'YYYY-MM-DD HH:MI:SS')", (DateTime)value);
        return this;
      }
    }
  }
}
