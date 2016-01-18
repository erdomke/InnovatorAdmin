using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin.Dialog
{
  public partial class ConnectionAdvancedDialog : FormBase
  {
    private BindingSource _bs;

    public Connections.ConnectionData ConnData
    {
      get { return (Connections.ConnectionData)_bs.Current; }
      set
      {
        _bs.DataSource = new List<Connections.ConnectionData>() { value };
        gridHeaders.DataSource = value.Params;
      }
    }

    public ConnectionAdvancedDialog()
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

      this.KeyPreview = true;
      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;

      gridHeaders.AutoGenerateColumns = false;

      _bs = new BindingSource();
      lblMessage.DataBindings.Add("Text", _bs, "ConnectionName");
      chkConfirm.DataBindings.Add("Checked", _bs, "Confirm");
      txtTimeout.DataBindings.Add("Text", _bs, "Timeout");

      colName.DataSource = new List<string>() { "LOCALE", "TIMEZONE_NAME" };
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      try
      {
        if (this.ValidateChildren())
        {
          this.DialogResult = System.Windows.Forms.DialogResult.OK;
          this.Close();
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void gridHeaders_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
    {
      try
      {
        if (e.ColumnIndex >= 0 && e.RowIndex >= 0
          && gridHeaders.Columns[e.ColumnIndex].DataPropertyName == "Value")
        {
          switch ((string)gridHeaders.Rows[e.RowIndex].Cells["colName"].Value)
          {
            case "LOCALE":
              colValue.DisplayMember = "DisplayName";
              colValue.ValueMember = "Name";
              colValue.DataSource = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                .OrderBy(v => v.DisplayName).ToList();
              break;
            case "TIMEZONE_NAME":
              colValue.DisplayMember = "DisplayName";
              colValue.ValueMember = "Id";
              colValue.DataSource = TimeZoneInfo.GetSystemTimeZones()
                .OrderBy(v => v.BaseUtcOffset.TotalHours).ThenBy(v => v.DisplayName).ToList();
              break;
            default:
              colValue.DataSource = new string[] {};
              break;
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
