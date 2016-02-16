using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Aras
{
  public class SelectColumn
  {
    public string ColumnName { get; set; }
    public string SubSelect { get; set; }

    public SelectColumn(string columnName) : this(columnName, null) { }
    public SelectColumn(string columnName, string subSelect)
    {
      this.ColumnName = columnName;
      this.SubSelect = subSelect;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
      {
        return false;
      }
      else if (obj is SelectColumn)
      {
        return Equals((SelectColumn)obj);
      }
      else
      {
        return false;
      }
    }
    public bool Equals(SelectColumn obj)
    {
      return this.ColumnName == obj.ColumnName && this.SubSelect == obj.SubSelect;
    }
    public override int GetHashCode()
    {
      return this.ColumnName.GetHashCode() ^ (this.SubSelect ?? "").GetHashCode();
    }
    public override string ToString()
    {
      return this.ColumnName + (string.IsNullOrEmpty(this.SubSelect) ? "" : "(" + this.SubSelect + ")");
    }
  }
}
