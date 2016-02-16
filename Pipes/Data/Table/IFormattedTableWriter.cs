using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface IFormattedTableWriter : ITableWriter, IConfigurable<TableWriterSettings>
  {
    IFormattedTableWriter Part(ReportParts part);
    IFormattedTableWriter PartEnd();
    IFormattedTableWriter Title(string name);
    IFormattedTableWriter Cell(ICell cell);
    IFormattedTableWriter Column(IColumn column);
  }
}
