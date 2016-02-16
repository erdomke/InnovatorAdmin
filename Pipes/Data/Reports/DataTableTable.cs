using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Pipes.Data.Reports
{
  public class DataTableTable : Table.BaseFormattedTable
  {
    private DataTable _table;
    private IList<Table.IColumn> _columns;

    public DataTableTable(DataTable table)
    {
      _table = table;
      _columns = (from c in table.Columns.Cast<DataColumn>()
                  select (Table.IColumn)new Table.Column(c.ColumnName)
                  {
                    Label = (string.IsNullOrEmpty(c.Caption) ? c.ColumnName : c.Caption),
                    Style = new Table.CellStyle(),
                    Visible = true
                  }).ToList();
    }

    public override IEnumerable<Table.IColumn> Columns
    {
      get  { return _columns; }
    }

    public override IEnumerable<Table.IRow> Rows
    {
      get 
      { 
        return (from r in _table.Rows.Cast<DataRow>()
                select (Table.IRow)new Table.Row(GetCells(r), r.Table.Columns.Count));
      }
    }

    private IEnumerable<Table.ICell> GetCells(DataRow item)
    {
      return item.ItemArray.Select((s, i) =>
        (Table.ICell)new Table.Cell(_columns[i])
        {
          FormattedValue = s,
          Value = s
        });
    }
  }
}
