using System.Data;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Pipes.Data.ParserMapping;

namespace Pipes.OfficeXml
{
  public class ExcelDataSetParser : IDataSetParser
  {
    public bool TryGetDataSet(System.IO.Stream stream, out DataSet ds)
    {
      ds = new DataSet();

      try
      {
        using (var doc = SpreadsheetDocument.Open(stream, false))
        {
          DataTable table;
          Worksheet ws;
          var shareStringPart = doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
          var strings = shareStringPart.SharedStringTable.Elements<SharedStringItem>().Select(ssi => ssi.InnerText).ToArray();
          DataRow dRow;

          foreach (var sheet in doc.WorkbookPart.Workbook.Descendants<Sheet>())
          {
            table = new DataTable(sheet.Name);
            ws = ((WorksheetPart)doc.WorkbookPart.GetPartById(sheet.Id)).Worksheet;

            // Make sure to add all 
            var columnNames = ws.Descendants<Row>()
                                .SelectMany(r => r.OfType<Cell>()
                                                  .Select(c => ExcelTableReader.IndexFromColumnName(c.CellReference)));
            if (columnNames.Any())
            {
              var maxColumn = columnNames.Max();
              for (int i = 1; i <= maxColumn; i++)
              {
                table.Columns.Add(ExcelTableReader.ColumnNameFromIndex(i), typeof(string));
              }

              uint lastRow = 0;

              foreach (var row in ws.Descendants<Row>())
              {
                // Add any missing rows into the table
                for (uint i = lastRow + 1; i < row.RowIndex; i++)
                {
                  table.Rows.Add(table.NewRow());
                }
                lastRow = row.RowIndex;

                dRow = table.NewRow();
                foreach (var cell in row.OfType<Cell>())
                {
                  dRow[GetColumnLetter(cell.CellReference)] = ExcelTableReader.GetCellValue(cell, strings);
                }
                table.Rows.Add(dRow);
              }

              ds.Tables.Add(table);
            }
          }
        }

        return true;
      }
      catch (FileFormatException)
      {
        return false;
      }
    }

    private string GetColumnLetter(string reference)
    {
      var i = 0;
      while (i < reference.Length && char.IsLetter(reference[i])) i++;
      return reference.Substring(0, i);
    }
  }
}
