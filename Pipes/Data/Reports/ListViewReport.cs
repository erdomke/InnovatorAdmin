using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pipes.Data.Reports
{
  public class ListViewReport : Table.IReport
  {
    public Table.IFormattedTable Footer { get; set; }
    public Table.IFormattedTable Header { get; set; }
    public string Name { get; set; }
    public Table.IFormattedTable Table { get; set; }

    public ListViewReport(ListView grid, string name) : this(grid, name, false) { }
    public ListViewReport(ListView grid, string name, bool selectedCellsOnly)
    {
      this.Name = name;
      this.Table = new ListViewTable(grid, selectedCellsOnly);
    }

    public static Table.IColumn GetColumn(ColumnHeader header)
    {
      return (Table.IColumn)new Table.Column(string.IsNullOrEmpty(header.Name) ? Guid.NewGuid().ToString() : header.Name)
                    {
                      Label = header.Text,
                      Style = GetStyle(null, header),
                      Visible = true,
                      Width = header.Width
                    };
    }

    private static Table.ICellStyle GetStyle(ListViewItem.ListViewSubItem cell, ColumnHeader header)
    {
      var result = new Table.CellStyle();
      if (header != null)
      {
        switch (header.TextAlign)
        {
          case HorizontalAlignment.Center:
            result.Alignment = Pipes.Data.Table.ContentAlignment.TopCenter;
            break;
          case HorizontalAlignment.Right:
            result.Alignment = Pipes.Data.Table.ContentAlignment.TopRight;
            break;
          default:
            result.Alignment = Pipes.Data.Table.ContentAlignment.TopLeft;
            break;
        }
      }
      if (cell != null)
      {
        result.BackColor = cell.BackColor;
        result.Font = new Table.FontStyle(cell.Font);
        result.ForeColor = cell.ForeColor;
      }
      return result;
    }

    private class ListViewTable : Table.BaseFormattedTable
    {
      private ListView _grid;
      private bool _selectedOnly;
      private IList<Table.IColumn> _columns;

      public ListViewTable(ListView grid, bool selectedCellsOnly)
      {
        _grid = grid;
        _selectedOnly = selectedCellsOnly;
        _columns = (from c in _grid.Columns.Cast<ColumnHeader>()
                    select GetColumn(c)).ToList();
      }

      public override IEnumerable<Table.IColumn> Columns
      {
        get 
        {
          return _columns;
        }
      }

      public override IEnumerable<Table.IRow> Rows
      {
        get 
        {
          return (from r in (_selectedOnly ? _grid.SelectedItems.Cast<ListViewItem>() : _grid.Items.Cast<ListViewItem>())
                  select (Table.IRow)new Table.Row(GetCells(r), r.SubItems.Count));
        }
      }

      private IEnumerable<Table.ICell> GetCells(ListViewItem item)
      {
        return item.SubItems.Cast<ListViewItem.ListViewSubItem>().Select((s, i) => 
          (Table.ICell)new Table.Cell(_columns[i], GetStyle(s, _grid.Columns[i])) {
            FormattedValue = s.Text,
            Value = s.Text
          });
      }
    }
  }
}
