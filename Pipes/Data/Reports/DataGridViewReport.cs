using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pipes.Data.Reports
{
  public class DataGridViewReport : Table.IReport
  {
    public Table.IFormattedTable Footer { get; set; }
    public Table.IFormattedTable Header { get; set; }
    public string Name { get; set; }
    public Table.IFormattedTable Table { get; set; }

    public DataGridViewReport(DataGridView grid, string name) : this(grid, name, null, false) { }
    public DataGridViewReport(DataGridView grid, string name, Action<Table.Cell, DataGridViewCell> processor, bool selectedCellsOnly)
    {
      this.Name = name;
      this.Table = new DataGridViewTable(grid, processor, selectedCellsOnly);
    }

    private static Table.ICellStyle GetStyle(DataGridViewCellStyle cellStyle)
    {
      return new Table.CellStyle() {
        Alignment = (Pipes.Data.Table.ContentAlignment)((int)cellStyle.Alignment),
        BackColor = cellStyle.BackColor,
        Font = new Table.FontStyle(cellStyle.Font),
        ForeColor = cellStyle.ForeColor,
        Format = cellStyle.Format,
        Padding = cellStyle.Padding,
        WrapMode = (Conversion.TriState)((int)cellStyle.WrapMode)
      };
    }

    private class DataGridViewTable : Table.BaseFormattedTable
    {
      private DataGridView _grid;
      private IList<Table.IColumn> _columns;
      private IList<Table.IRow> _rows;
      private Action<Table.Cell, DataGridViewCell> _processor;

      public DataGridViewTable(DataGridView grid, Action<Table.Cell, DataGridViewCell> processor, bool selectedCellsOnly)
      {
        _grid = grid;
        _processor = processor;
        var cellsToProcess = (selectedCellsOnly ? (from c in _grid.SelectedCells.Cast<DataGridViewCell>()
                                                   orderby c.RowIndex, c.OwningColumn.DisplayIndex
                                                   select c).ToList() 
                                                : null);

        _columns = (from c in (cellsToProcess == null
                                 ? _grid.Columns.Cast<DataGridViewColumn>()
                                 : cellsToProcess.Select(c => c.OwningColumn).Distinct().OrderBy(c => c.DisplayIndex))
                    orderby c.DisplayIndex
                    select (Table.IColumn)new Table.Column(c.DataPropertyName ?? c.Name)
                    {
                      Style = GetStyle(c.HeaderCell.InheritedStyle),
                      Label = c.HeaderText,
                      Visible = c.Visible,
                      Width = c.Width
                    }).ToList();

        if (cellsToProcess != null)
        {
          var cells = new List<Table.ICell>(_columns.Count);
          _rows = new List<Table.IRow>((int)Math.Ceiling((double)cellsToProcess.Count / _columns.Count));
          var colIndex = -1;
          for (var i = 0; i < cellsToProcess.Count; i++)
          {
            if (cells.Any() && i > 0 && 
                cellsToProcess[i].RowIndex != cellsToProcess[i-1].RowIndex)
            {
              colIndex = 0;
              _rows.Add(new Table.Row(cells.ToArray()));
              cells.Clear();
            }
            else 
            {
              colIndex++;
            }

            while(colIndex < _columns.Count && 
                  _columns[colIndex].Name != (cellsToProcess[i].OwningColumn.DataPropertyName ?? 
                                              cellsToProcess[i].OwningColumn.Name))
            {
              cells.Add(new Table.Cell(_columns[colIndex]));
              colIndex++;
            }
            if (colIndex >= _columns.Count) 
              throw new InvalidOperationException(string.Format("Column '{0}' could not be found for cell.", 
                (cellsToProcess[i].OwningColumn.DataPropertyName ?? 
                 cellsToProcess[i].OwningColumn.Name)));

            cells.Add(GetCell(cellsToProcess[i]));
          }
          _rows.Add(new Table.Row(cells.ToArray()));
        }

        this.DefaultStyle = GetStyle(grid.DefaultCellStyle);
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
          if (_rows == null)
          {
            return _grid.Rows.Cast<DataGridViewRow>().Select(
              r => (Table.IRow)new Table.Row((from c in r.Cells.Cast<DataGridViewCell>()
                                              orderby c.OwningColumn.DisplayIndex
                                              select GetCell(c)), r.Cells.Count)
            );
          }
          else
          {
            return _rows;            
          }
        }
      }

      private Table.ICell GetCell(DataGridViewCell cell)
      {
        var result = new Table.Cell(cell.OwningColumn.DataPropertyName ?? cell.OwningColumn.Name, GetStyle(cell.InheritedStyle));
        try
        {
          result.FormattedValue = cell.FormattedValue;
        }
        catch (Exception)
        {
          result.FormattedValue = cell.Value;
        }
        result.Value = cell.Value;

        if (_processor != null)
        {
          _processor.Invoke(result, cell);
        }

        return result;
      }
    }
  }
}
