using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Pipes.Data.Reports
{
  public class DataTableReport : Table.IReport
  {
    public Table.IFormattedTable Footer { get; set; }
    public Table.IFormattedTable Header { get; set; }
    public string Name { get; set; }
    public Table.IFormattedTable Table { get; set; }

    public DataTableReport(DataTable table) : this(table, table.TableName) { }
    public DataTableReport(DataTable table, string name) 
    {
      this.Name = name;
      this.Table = new DataTableTable(table);
    }

  }
}
