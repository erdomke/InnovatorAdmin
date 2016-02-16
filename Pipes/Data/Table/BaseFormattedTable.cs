using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public abstract class BaseFormattedTable : IFormattedTable
  {
    public Table.ICellStyle DefaultStyle { get; set; }

    public abstract IEnumerable<Table.IColumn> Columns { get; }
    public abstract IEnumerable<Table.IRow> Rows { get; }

    IEnumerable<Table.IColumnMetadata> Table.ITable.Columns
    {
      get { return this.Columns.OfType<Table.IColumnMetadata>(); }
    }

    public IEnumerator<IDataRecord> GetEnumerator()
    {
      return this.Rows.Cast<IDataRecord>().GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }
  }
}
