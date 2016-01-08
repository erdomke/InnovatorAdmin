using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  public partial class FilterSelect<T> : FormBase
  {
    private IEnumerable<T> _dataSource;
    private FullBindingList<T> _filterable;
    private PropertyDescriptorCollection _props;

    public IEnumerable<T> DataSource
    {
      get { return _dataSource; }
      set
      {
        _dataSource = value;
        var filterable = new FullBindingList<T>();
        filterable.AddRange(_dataSource);
        listValues.DataSource = filterable;
        _filterable = filterable;
        SortDefault();
      }
    }
    public string DisplayMember
    {
      get { return listValues.DisplayMember; }
      set { listValues.DisplayMember = value; }
    }
    public string Message
    {
      get { return lblMessage.Text; }
      set { lblMessage.Text = value; }
    }
    public T SelectedItem { get { return (T)listValues.SelectedItem; } }

    public FilterSelect()
    {
      InitializeComponent();

      this.TitleLabel = pnlTitle;
      this.LeftBorderPanel = pnlLeft;
      this.TopLeftCornerPanel = pnlTopLeft;
      this.TopBorderPanel = pnlTop;
      this.TopRightCornerPanel = pnlTopRight;
      this.RightBorderPanel = pnlRight;
      this.BottomRightCornerPanel = pnlBottomRight;
      this.BottomBorderPanel = pnlBottom;
      this.BottomLeftCornerPanel = pnlBottomLeft;
      this.InitializeTheme();

      _props = TypeDescriptor.GetProperties(typeof(T));
      this.KeyPreview = true;
      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
      if (listValues.ContainsFocus) txtFilter.Focus();
      base.OnKeyDown(e);
    }


    private void txtFilter_TextChanged(object sender, EventArgs e)
    {
      try
      {
        _filterable.ApplyFilter(v => GetDisplayText(v).IndexOf(txtFilter.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        SortDefault();
        if (_filterable.Count > 0) listValues.SelectedIndex = 0;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void SortDefault()
    {
      _filterable.ApplySort((x, y) =>
      {
        var xText = GetDisplayText(x);
        var yText = GetDisplayText(y);
        var compare = SortGroup(xText).CompareTo(SortGroup(yText));
        if (compare == 0) compare = xText.CompareTo(yText);
        return compare;
      });
    }

    private int SortGroup(string value)
    {
      if (value.StartsWith(txtFilter.Text, StringComparison.OrdinalIgnoreCase)) return 1;
      return 2;
    }

    private string GetDisplayText(T value)
    {
      if (value == null)
      {
        return string.Empty;
      }
      else if (string.IsNullOrEmpty(this.DisplayMember))
      {
        return value.ToString();
      }
      else
      {
        return _props[this.DisplayMember].GetValue(value).ToString();
      }
    }

    private void txtFilter_KeyDown(object sender, KeyEventArgs e)
    {
      try
      {
        var page = listValues.Height / listValues.ItemHeight;
        switch (e.KeyCode)
        {
          case Keys.Down:
            if (_filterable.Count > 1)
            {
              if (listValues.SelectedIndex < 0)
              {
                listValues.SelectedIndex = 0;
              }
              else if (listValues.SelectedIndex < (listValues.Items.Count - 1))
              {
                listValues.SelectedIndex += 1;
              }
            }
            e.Handled = true;
            break;
          case Keys.Up:
            if (listValues.SelectedIndex > 0)
            {
              listValues.SelectedIndex -= 1;
            }
            e.Handled = true;
            break;
          case Keys.PageDown:
            listValues.SelectedIndex = Math.Min(Math.Max(0, listValues.SelectedIndex) + page, listValues.Items.Count - 1);
            e.Handled = true;
            break;
          case Keys.PageUp:
            listValues.SelectedIndex = Math.Max(Math.Max(0, listValues.SelectedIndex) - page, 0);
            e.Handled = true;
            break;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void listValues_KeyDown(object sender, KeyEventArgs e)
    {
      try
      {
        txtFilter.Focus();

      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void listValues_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      try
      {
        this.DialogResult = System.Windows.Forms.DialogResult.OK;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    public DialogResult ShowDialog(IWin32Window owner, Rectangle bounds)
    {
      if (bounds != default(Rectangle))
      {
        this.StartPosition = FormStartPosition.Manual;
        var screenDim = SystemInformation.VirtualScreen;
        var newX = Math.Min(Math.Max(bounds.X, 0), screenDim.Width - this.DesktopBounds.Width);
        var newY = Math.Min(Math.Max(bounds.Y - 30, 0), screenDim.Height - this.DesktopBounds.Height);
        this.DesktopLocation = new Point(newX, newY);
      }
      return this.ShowDialog(owner);
    }
  }
}
