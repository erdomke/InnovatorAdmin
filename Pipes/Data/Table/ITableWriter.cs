using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface ITableWriter
  {
    void Flush();
    ITableWriter Cell(string name, object value);
    ITableWriter Column(IColumnMetadata column);
    ITableWriter Head();
    ITableWriter HeadEnd();
    ITableWriter Row();
    ITableWriter RowEnd();
  }
}
