using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes;
using Pipes.Data;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Pipes.OfficeXml
{
  public class ExcelTableReader : IPipeInput<System.IO.Stream>, IPipeOutput<IDataRecord>
  {
    private int _startCol = 1;
    private int _startRow = 1;
    private int _endCol = 1;
    private int _endRow = 1;

    public bool FirstRowColumnNames { get; set; }
    public string Sheet { get; set; }
    private IEnumerable<System.IO.Stream> _source;

    public ExcelTableReader SetRange(int startCol, int startRow, int endCol, int endRow)
    {
      _startCol = startCol;
      _startRow = startRow;
      _endCol = endCol;
      _endRow = endRow;
      return this;
    }
    public ExcelTableReader SetSheet(string name)
    {
      this.Sheet = name;
      return this;
    }

    public void Initialize(IEnumerable<System.IO.Stream> source)
    {
      _source = source;
    }

    public IEnumerator<IDataRecord> GetEnumerator()
    {
      string[] columns = null;
      object[] values = null;
      int colCount = _endCol - _startCol + 1;

      foreach (var stream in _source)
      {
        using (var doc = SpreadsheetDocument.Open(stream, false))
        {
          var sheet = doc.WorkbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == this.Sheet).SingleOrDefault();
          if (sheet == null) yield break;
          var ws = ((WorksheetPart)doc.WorkbookPart.GetPartById(sheet.Id)).Worksheet;
          var shareStringPart = doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
          var strings = shareStringPart.SharedStringTable.Elements<SharedStringItem>().Select(ssi => ssi.InnerText).ToArray();

          if (_endCol == 0 && _startCol == 0 || _startRow == 0 && _endRow == 0)
          {
            //ws.SheetDimension.Reference.Value;
          }

          foreach (var row in ws.Descendants<Row>().Where(r => r.RowIndex.Value >= _startRow && r.RowIndex.Value <= _endRow))
          {
            if (columns == null && this.FirstRowColumnNames)
            {
              columns = new string[colCount];
              foreach (var cell in row.OfType<Cell>().Where(c => IndexFromColumnName(c.CellReference.Value) >= _startCol && IndexFromColumnName(c.CellReference.Value) <= _endCol))
              {
                columns[IndexFromColumnName(cell.CellReference.Value) - _startCol] = (GetCellValue(cell, strings) ?? "").ToString();
              }
            }
            else
            {
              values = new object[colCount];
              foreach (var cell in row.OfType<Cell>().Where(c => IndexFromColumnName(c.CellReference.Value) >= _startCol && IndexFromColumnName(c.CellReference.Value) <= _endCol))
              {
                values[IndexFromColumnName(cell.CellReference.Value) - _startCol] = GetCellValue(cell, strings);
              }
              yield return new DataRecord(values, columns);
            }
          }

          doc.Close();
        }
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public static DateTime ExtractDate(object value)
    {
      if (value == null) throw new ArgumentNullException();
      double dblValue;
      if (value is DateTime)
      {
        return (DateTime)value;
      }
      else if (value is int)
      {
        dblValue = (double)(int)value;
      }
      else if (value is double)
      {
        dblValue = (double)value;
      }
      else
      {
        var strValue = value.ToString();
        if (!double.TryParse(strValue, out dblValue))
        {
          return DateTime.Parse(strValue);
        }
      }
      return DateTime.FromOADate(dblValue);
    }

    internal static object GetCellValue(Cell cell, string[] sharedStrings)
    {
      if (cell == null) return null;
      if (cell.DataType == null)
      {
        if (cell.CellValue == null || string.IsNullOrEmpty(cell.CellValue.Text)) return null;
        return cell.CellValue.Text;
      }

      switch (cell.DataType.Value ) {
        case CellValues.Boolean:
          if (cell.CellValue == null) return false;
          return cell.CellValue.Text != "0";
        case CellValues.Date:
          if (cell.CellValue == null || string.IsNullOrEmpty(cell.CellValue.Text)) return null;
          return ExtractDate(cell.CellValue.Text);
        case CellValues.InlineString:
          if (cell.CellValue == null || string.IsNullOrEmpty(cell.CellValue.Text)) return null;
          return cell.CellValue.Text;
        case CellValues.Number:
          if (cell.CellValue == null || string.IsNullOrEmpty(cell.CellValue.Text)) return null;
          return double.Parse(cell.CellValue.Text);
        case CellValues.SharedString:
          if (cell.CellValue == null || string.IsNullOrEmpty(cell.CellValue.Text)) return null;
          return sharedStrings[int.Parse(cell.CellValue.Text)];
      }
      return null;
    }

    internal static int IndexFromColumnName(string name)
    {
      int value = 0;
      foreach (var chr in name)
      {
        if (Char.IsLetter(chr))
        {
          if (value > 0) value *= 26;
          value += (int)Char.ToUpperInvariant(chr) - (int)'A' + 1;
        }
      }
      return value;
    }
    internal static string ColumnNameFromIndex(int value)
    {
      string result = null;

      value--;
      while (value > 0 || (value == 0 && result == null))
      {
        if (string.IsNullOrEmpty(result))
        {
          result = ((char)(value % 26 + 65)).ToString();
        }
        else
        {
          result = ((char)(value % 26 - 1 + 65)).ToString() + result;
        }
        value = value / 26;
      }
      return result;
    }
  }
}
