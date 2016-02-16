using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface IRow : IDataRecord
  {
    IEnumerable<ICell> Cells { get; }
  }
}
