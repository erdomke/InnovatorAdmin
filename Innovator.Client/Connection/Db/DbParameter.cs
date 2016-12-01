#if DBDATA
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  public class DbParameter : IDbDataParameter
  {
    public string ParameterName { get; set; }
    public object Value { get; set; }

    public DbParameter() { }
    public DbParameter(string name, object value)
    {
      this.ParameterName = name;
      this.Value = value;
    }

    byte IDbDataParameter.Precision { get; set; }
    byte IDbDataParameter.Scale { get; set; }
    int IDbDataParameter.Size { get; set; }
    DbType IDataParameter.DbType { get; set; }
    ParameterDirection IDataParameter.Direction { get; set; }
    bool IDataParameter.IsNullable
    {
      get { return true; }
    }
    string IDataParameter.SourceColumn { get; set; }
    DataRowVersion IDataParameter.SourceVersion { get; set; }
  }
}
#endif
