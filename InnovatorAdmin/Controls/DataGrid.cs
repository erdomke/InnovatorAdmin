using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Pipes;

namespace InnovatorAdmin.Controls
{
  public class DataGrid : DataGridView
  {
    public DataGrid()
    {
      this.BackgroundColor = System.Drawing.Color.White;
      this.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;

      using (var g = this.CreateGraphics())
      {
        var dpi = g.DpiX;
        if (this.RowTemplate.Height <= 22 && dpi > 96)
        {
          this.RowTemplate.Height = (int)((double)this.RowTemplate.Height * dpi / 96.0);
        }
      }

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

    public IEnumerable<DataGridViewRow> GetSelectedRows()
    {
      var rows = this.SelectedRows.OfType<DataGridViewRow>();
      if (!rows.Any())
        rows = this.SelectedCells.OfType<DataGridViewCell>().Select(c => c.OwningRow).Distinct();
      if (!rows.Any())
        rows = Enumerable.Repeat(this.CurrentCell.OwningRow, 1);
      return rows;
    }

    public override DataObject GetClipboardContent()
    {
      return base.GetClipboardContent();
    }

    public void Paste()
    {
      if (this.IsCurrentCellInEditMode)
      {
        // do nothing
      }
      else if (!string.IsNullOrEmpty(Clipboard.GetText()))
      {
        var reader = new Pipes.IO.StringTextSource(Clipboard.GetText().TrimEnd('\r', '\n'))
          .Pipe(new Pipes.Data.DelimitedTextLineReader());
        reader.AddDelim('\t');
        reader.FieldEnclosingChar = '"';
        reader.EnclosingCharEscape = '"';

        var lines = reader.ToArray();
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
              var maxRows = this.AllowUserToAddRows ? clipRows : Math.Min(clipRows, this.RowCount - row);
              for (var pasteRow = 0; pasteRow < maxRows; pasteRow++)
              {
                fields = lines[pasteRow].ToArray();
                if (this.Rows[row + pasteRow].IsNewRow)
                {
                  for (var pRow = pasteRow; pRow < maxRows; pRow++)
                  {
                    fields = lines[pRow].ToArray();
                    var newRow = CreateRow();
                    for (var pasteCol = 0; pasteCol < Math.Min(fields.Length, visColumns.Length - colIdx); pasteCol++)
                    {
                      cell = this[visColumns[colIdx + pasteCol], row + pasteRow];
                      if (!cell.ReadOnly)
                      {
                        try
                        {
                          newRow[this.Columns[visColumns[colIdx + pasteCol]].DataPropertyName]
                            = cell.ParseFormattedValue(fields[pasteCol], cell.Style, null, null);
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
                    newRow.Add();
                  }

                  if (pasteRow == 0)
                    this.Rows.RemoveAt(this.RowCount - 2);

                  return;
                }
                else
                {
                  for (var pasteCol = 0; pasteCol < Math.Min(fields.Length, visColumns.Length - colIdx); pasteCol++)
                  {
                    if ((row + pasteRow) < selectMatrix.Count)
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
        }

        if (pasteErrorCount > 0)
        {
          var pasteError = new Exception(string.Format("Could not paste {0} values. For example:\r\n{1}", pasteErrorCount, pasteErrors.ToString()));
          Utils.HandleError(pasteError);
        }
      }
    }

    private interface INewRow
    {
      object this[string name] { get; set; }
      void Add();
    }

    private class NewTableRow : INewRow
    {
      private DataRow _row;

      public NewTableRow(DataTable table)
      {
        _row = table.NewRow();
      }

      public object this[string name]
      {
        get { return _row[name]; }
        set { _row[name] = value; }
      }

      public void Add()
      {
        _row.Table.Rows.Add(_row);
      }
    }

    private INewRow CreateRow()
    {
      var tbl = this.DataSource as DataTable;
      if (tbl == null)
        throw new NotSupportedException();
      return new NewTableRow(tbl);
    }
  }
}
