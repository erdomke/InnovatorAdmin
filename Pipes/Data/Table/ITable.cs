using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface ITable : IPipeOutput<IDataRecord>
  {
    IEnumerable<IColumnMetadata> Columns { get; }
  }
}
