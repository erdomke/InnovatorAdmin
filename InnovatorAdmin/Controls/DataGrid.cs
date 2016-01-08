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
    protected override void OnKeyDown(KeyEventArgs e)
    {
      try
      {
        if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
        {
          Paste();
        }
        else
        {
          base.OnKeyDown(e);
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

    public void Paste()
    {
      if (this.IsCurrentCellInEditMode)
      {
        // do nothing
      }
      else if (!string.IsNullOrEmpty(Clipboard.GetText())) 
      {
	      var lines = Clipboard.GetText().TrimEnd('\r', '\n').Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
	      var clipRows = lines.Length;
	      string[] fields = null;
        DataGridViewCell cell;
        var visColumns = Enumerable.Range(0, this.ColumnCount)
          .Where(c => this.Columns[c].Visible)
          .OrderBy(c => this.Columns[c].DisplayIndex).ToArray();
	      var selectMatrix = (from r in Enumerable.Range(0, this.RowCount)
                            select (from c in visColumns
                                    select this[c, r].Selected).ToList()).ToList();
        var pasteErrorCount = 0;
        var pasteErrors = new StringBuilder();

	      for (var row = 0; row <= this.RowCount - 1; row++) 
        {
          for (var colIdx = 0; colIdx < visColumns.Length; colIdx++)
          {
            if (selectMatrix[row][colIdx])
            {
              for (var pasteRow = 0; pasteRow <= Math.Min(clipRows, this.RowCount - row) - 1; pasteRow++)
              {
                fields = lines[pasteRow].Split('\t');
                for (var pasteCol = 0; pasteCol <= Math.Min(fields.Length, visColumns.Length - colIdx) - 1; pasteCol++)
                {
                  selectMatrix[row + pasteRow][colIdx + pasteCol] = false; // Prevent overwriting paste area
                  cell = this[visColumns[colIdx + pasteCol], row + pasteRow];
                  if (!cell.ReadOnly)
                  {
                    try
                    {
                      cell.Value = cell.ParseFormattedValue(fields[pasteCol], cell.Style, null, null);
                    }
                    catch (FormatException)
                    {
                      if (pasteErrorCount < 3)
                      {
                        pasteErrors.AppendFormat("  '{0}' could not be pasted in {1} of type {2}"
                          , fields[pasteCol]
                          , cell.OwningColumn.HeaderText ?? cell.OwningColumn.Name
                          , cell.OwningColumn.ValueType.Name).AppendLine();
                      }
                      pasteErrorCount++;
                    }
                  }
                }
              }
            }
          }
	      }

        if (pasteErrorCount > 0)
        {
          var pasteError = new Exception(string.Format("Could not paste {0} values. For example:\r\n{1}", pasteErrorCount, pasteErrors.ToString()));
          Utils.HandleError(pasteError);
        }
	    }
    }
  }
}
