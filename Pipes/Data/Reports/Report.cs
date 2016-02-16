using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Reports
{
  public class Report : Table.IReport
  {
    public Table.IFormattedTable Footer { get; set; }
    public Table.IFormattedTable Header { get; set; }
    public string Name { get; set; }
    public Table.IFormattedTable Table { get; set; }
  }
}
