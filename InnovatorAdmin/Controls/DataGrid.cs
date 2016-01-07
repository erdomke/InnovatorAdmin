using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  public class DataGrid : DataGridView
  {
    public DataGrid()
    {
      this.BackgroundColor = System.Drawing.Color.White;
      this.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
    }

    protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
    {
      base.OnCellFormatting(e);
      try
      {
        if (e.Value == e.CellStyle.DataSourceNullValue)
        {
          e.CellStyle.BackColor = Color.AntiqueWhite;
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
    {
      Utils.HandleError(e.Exception);
    }
  }
}
