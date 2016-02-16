using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface IFormattedTable : ITable 
  {
    new IEnumerable<IColumn> Columns { get; }
    IEnumerable<IRow> Rows { get; }
  }
}
