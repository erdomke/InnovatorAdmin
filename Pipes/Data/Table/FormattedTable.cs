using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public class FormattedTable : IFormattedTable
  {
    private IList<IColumn> _columns = new List<IColumn>();
    private IList<IRow> _rows = new List<IRow>();

    public IList<IColumn> Columns
    {
      get { return _columns; }
    }
    public IList<IRow> Rows
    {
      get { return _rows; }
    }



    IEnumerable<IColumn> IFormattedTable.Columns
    {
      get { return _columns; }
    }

    IEnumerable<IRow> IFormattedTable.Rows
    {
      get { return _rows; }
    }

    IEnumerable<IColumnMetadata> ITable.Columns
    {
      get { return _columns.Cast<IColumnMetadata>(); }
    }

    IEnumerator<IDataRecord> IEnumerable<IDataRecord>.GetEnumerator()
    {
      return _rows.Cast<IDataRecord>().GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return _rows.Cast<IDataRecord>().GetEnumerator();
    }
  }
}
