using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  internal class DataGridViewSelection
  {
    private string _columnPropertyName = null;
    private IEnumerable<DataGridViewRow> _rows;

    public string ColumnPropertyName { get { return _columnPropertyName; } }
    public IEnumerable<DataGridViewRow> Rows { get { return _rows; } }

    public DataGridViewSelection(DataGridView grid)
    {
      _rows = grid.SelectedRows.OfType<DataGridViewRow>();
      if (!_rows.Any())
      {
        _rows = grid.SelectedCells.OfType<DataGridViewCell>().Select(c => c.OwningRow).Distinct();
        var cols = grid.SelectedCells.OfType<DataGridViewCell>().Select(c => c.OwningColumn).Distinct().ToArray();
        if (cols.Length == 1)
          _columnPropertyName = cols[0].DataPropertyName;
      }
      if (!_rows.Any())
      {
        _rows = Enumerable.Repeat(grid.CurrentCell.OwningRow, 1);
        _columnPropertyName = grid.CurrentCell.OwningColumn.DataPropertyName;
      }
      var client = grid.PointToClient(Cursor.Position);
      var hit = grid.HitTest(client.X, client.Y);
      if (!_rows.Any(r => r.Index == hit.RowIndex))
      {
        if (hit.RowIndex >= 0)
        {
          _rows = Enumerable.Repeat(grid.Rows[hit.RowIndex], 1);
          _columnPropertyName = grid.Columns[hit.ColumnIndex].DataPropertyName;
        }
        else
        {
          _rows = Enumerable.Empty<DataGridViewRow>();
          _columnPropertyName = null;
        }
      }
    }
  }
}
