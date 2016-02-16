using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface IQueryableTable : ITable
  {
    void AddColumnFilter(Func<IColumnMetadata, bool> criteria);
    void AddRowFilter(Func<IDataRecord, bool> criteria);
    bool ContainsColumnName(string name);
  }
}
