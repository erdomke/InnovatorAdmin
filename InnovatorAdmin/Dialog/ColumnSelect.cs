using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin.Dialog
{
  public partial class ColumnSelect : FormBase
  {
    private DataGridView _dataSource;
    private FullBindingList<ColWrapper> _binding;

    public DataGridView DataSource
    {
      get { return _dataSource; }
      set
      {
        _dataSource = value;
        _binding = new FullBindingList<ColWrapper>(_dataSource.Columns.OfType<DataGridViewColumn>().Select(c => new ColWrapper(c)));
        _binding.ApplySort("HeaderText");
        grid.DataSource = _binding;
      }
    }

    private class ColWrapper
    {
      private DataGridViewColumn _col;

      public ColWrapper(DataGridViewColumn col)
      {
        _col = col;
      }

      public string DataPropertyName { get { return _col.DataPropertyName; } set { _col.DataPropertyName = value; } }
      public int DisplayIndex { get { return _col.DisplayIndex + 1; } set { _col.DisplayIndex = value - 1; } }
      public string HeaderText { get { return _col.HeaderText; } set { _col.HeaderText = value; } }
      public bool Visible { get { return _col.Visible; } set { _col.Visible = value; } }

      public override bool Equals(object obj)
      {
        var wrapper = obj as ColWrapper;
        if (wrapper == null)
          return false;
        return Equals(wrapper);
      }
      public bool Equals(ColWrapper obj)
      {
        return _col.Equals(obj._col);
      }
      public override int GetHashCode()
      {
        return _col.GetHashCode();
      }
    }

    public string Message
    {
      get { return lblMessage.Text; }
      set { lblMessage.Text = value; }
    }

    public ColumnSelect()
    {
      InitializeComponent();

      this.TitleLabel = lblTitle;
      this.TopLeftCornerPanel = pnlTopLeft;
      this.TopBorderPanel = pnlTop;
      this.TopRightCornerPanel = pnlTopRight;
      this.LeftBorderPanel = pnlLeft;
      this.RightBorderPanel = pnlRight;
      this.BottomRightCornerPanel = pnlBottomRight;
      this.BottomBorderPanel = pnlBottom;
      this.BottomLeftCornerPanel = pnlBottomLeft;
      this.InitializeTheme();

      InitializeDpi();

      this.KeyPreview = true;
      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;
      grid.AutoGenerateColumns = false;
      grid.RowTemplate.Height = (int)(DpiScale * 22);

      btnMoveUp.Font = FontAwesome.Font;
      btnMoveUp.Text = FontAwesome.Fa_arrow_up.ToString();
      btnMoveDown.Font = FontAwesome.Font;
      btnMoveDown.Text = FontAwesome.Fa_arrow_down.ToString();
    }

    private void btnMoveUp_Click(object sender, EventArgs e)
    {
      try
      {
        var rows = grid.GetSelectedRows();
        var cols = rows.Select(r => (ColWrapper)r.DataBoundItem).OrderBy(c => c.DisplayIndex).ToArray();

        foreach (var col in cols)
        {
          var prev = _binding.FirstOrDefault(c => c.DisplayIndex == col.DisplayIndex - 1);
          if (prev != null && !cols.Contains(prev))
          {
            col.DisplayIndex--;
          }
        }
        RefreshGrid(cols, true);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnMoveDown_Click(object sender, EventArgs e)
    {
      try
      {
        var rows = grid.GetSelectedRows();
        var cols = rows.Select(r => (ColWrapper)r.DataBoundItem).OrderByDescending(c => c.DisplayIndex).ToArray();

        foreach (var col in cols)
        {
          var next = _binding.FirstOrDefault(c => c.DisplayIndex == col.DisplayIndex + 1);
          if (next != null && !cols.Contains(next))
          {
            col.DisplayIndex++;
          }
        }

        RefreshGrid(cols, false);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void RefreshGrid(IEnumerable<ColWrapper> selectItems, bool topDown)
    {
      var currSort = _binding.SortDescriptions;
      _binding.ResetBindings();
      if (currSort.Count > 0)
        _binding.ApplySort(currSort);

      bool currCellSet = false;
      foreach (var row in grid.Rows.OfType<DataGridViewRow>()
                              .OrderBy(r => r.Index * (topDown ? 1 : -1))
                              .Where(r => selectItems.Contains(r.DataBoundItem)))
      {
        if (!currCellSet)
        {
          grid.CurrentCell = grid[0, row.Index];
          currCellSet = true;
        }
        row.Selected = true;
      }
    }
  }
}
