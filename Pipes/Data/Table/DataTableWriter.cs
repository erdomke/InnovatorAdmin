using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Pipes.Data.Table
{
  public class DataTableWriter : ITableWriter
  {
    private DataRow _row;
    private DataTable _table;

    public DataTable Table 
    {
      get
      {
        return _table;
      }
    }

    public void Flush()
    {
      _table.EndInit();
      _table.AcceptChanges();
    }

    public ITableWriter Cell(string name, object value)
    {
      if (value == null)
      {
        _row[name] = DBNull.Value;
      }
      else
      {
        _row[name] = value;
      }
      return this;
    }

    public ITableWriter Column(IColumnMetadata column)
    {
      var col = _table.Columns.Add(column.Name, column.DataType);
      if (!string.IsNullOrEmpty(column.Label)) col.Caption = column.Label;
      return this;
    }

    public ITableWriter Head()
    {
      _table = new DataTable();
      return this;
    }

    public ITableWriter HeadEnd()
    {
      _table.BeginInit();
      return this;
    }

    public ITableWriter Row()
    {
      _row = _table.NewRow();
      _row.BeginEdit();
      return this;
    }

    public ITableWriter RowEnd()
    {
      _row.EndEdit();
      _table.Rows.Add(_row);
      return this;
    }
  }
}
