using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;

namespace InnovatorAdmin
{
  public class ExcelReader : IEnumerable<object[]>, IDisposable //: IPipeInput<System.IO.Stream>, IPipeOutput<IDataRecord>
  {
    private int _colCount;
    private string[] _columns = null;
    private SpreadsheetDocument _doc;
    private int _endCol = -1;
    private int _endRow = -1;
    private int _startCol = -1;
    private int _startRow = -1;

    /// <summary>
    /// The names of all columns as read from the first row (if so configured)
    /// </summary>
    public string[] Columns
    {
      get
      {
        if (_columns == null && this.FirstRowColumnNames)
        {
          // Trigger the enumeration to progress far enough forward to read the column name row
          this.First();
        }
        return _columns;
      }
    }
    /// <summary>
    /// Number of columns
    /// </summary>
    public int ColumnCount
    {
      get { return _colCount; }
    }
    /// <summary>
    /// Whether the first row contains the column names
    /// </summary>
    public bool FirstRowColumnNames { get; set; }
    /// <summary>
    /// The name of the sheet to pull data from
    /// </summary>
    public string Sheet { get; set; }
    /// <summary>
    /// The names of all sheets in the spreadsheet
    /// </summary>
    public IEnumerable<string> SheetNames
    {
      get { return _doc.WorkbookPart.Workbook.Descendants<Sheet>().Select(s => s.Name.Value); }
    }

    /// <summary>
    /// Sets the range of cells to read data from using 1-based indices
    /// </summary>
    /// <param name="startCol">One-based index of the first column to read</param>
    /// <param name="startRow">One-based index of the first row to read</param>
    /// <param name="endCol">One-based index of the last column to read (inclusive, i.e. this column is read)</param>
    /// <param name="endRow">One-based index of the last row to read (inclusive, i.e. this row is read)</param>
    public ExcelReader SetRange(int startCol, int startRow, int endCol, int endRow)
    {
      _startCol = startCol;
      _startRow = startRow;
      _endCol = endCol;
      _endRow = endRow;
      _colCount = _endCol - _startCol + 1;
      return this;
    }
    /// <summary>
    /// Sets the name of the sheet to read from
    /// </summary>
    /// <param name="name">Name of the sheet</param>
    public ExcelReader SetSheet(string name)
    {
      this.Sheet = name;
      return this;
    }

    /// <summary>
    /// Sets the stream of data to read from
    /// </summary>
    /// <param name="source">Stream of data</param>
    public void Initialize(System.IO.Stream source)
    {
      _doc = SpreadsheetDocument.Open(source, false);
    }

    public IEnumerator<object[]> GetEnumerator()
    {
      _columns = null;
      object[] values = null;
      _colCount = _endCol - _startCol + 1;

      Sheet sheet;
      if (string.IsNullOrEmpty(this.Sheet))
      {
        sheet = _doc.WorkbookPart.Workbook.Descendants<Sheet>().Where(s => s.SheetId.Value == 1).FirstOrDefault();
        if (sheet == null) sheet = _doc.WorkbookPart.Workbook.Descendants<Sheet>().First();
      }
      else
      {
        sheet = _doc.WorkbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == this.Sheet).SingleOrDefault();
      }

      if (sheet == null) yield break;
      var ws = ((WorksheetPart)_doc.WorkbookPart.GetPartById(sheet.Id)).Worksheet;
      var shareStringPart = _doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().First();
      var strings = shareStringPart.SharedStringTable.Elements<SharedStringItem>().Select(ssi => ssi.InnerText).ToArray();
      int colIndex;
      Row row;

      var reader = OpenXmlReader.Create(ws);
      while (reader.Read())
      {
        if (reader.ElementType == typeof(Columns) && reader.IsStartElement)
        {
          var colDefns = (Columns)reader.LoadCurrentElement();

          if (_startCol < 0 || _endCol < 0)
          {
            _startCol = (int)colDefns.ChildElements.OfType<Column>().Select(c => c.Min.Value).Min();
            _endCol = (int)colDefns.ChildElements.OfType<Column>().Select(c => c.Max.Value).Max();
          }
          else
          {
            _startCol = Math.Max(_startCol, (int)colDefns.ChildElements.OfType<Column>().Select(c => c.Min.Value).Min());
            _endCol = Math.Min(_endCol, (int)colDefns.ChildElements.OfType<Column>().Select(c => c.Max.Value).Max());
          }
          _colCount = _endCol - _startCol + 1;
        }
        else if (reader.ElementType == typeof(Row) && reader.IsStartElement)
        {
          row = (Row)reader.LoadCurrentElement();

          if ((_startRow < 0 || row.RowIndex.Value >= _startRow) && (_endRow < 0 || row.RowIndex.Value <= _endRow))
          {
            if (_columns == null && this.FirstRowColumnNames)
            {
              _columns = new string[_colCount];
              foreach (var cell in row.OfType<Cell>())
              {
                colIndex = IndexFromColumnName(cell.CellReference.Value);
                if (colIndex >= _startCol && colIndex <= _endCol)
                {
                  _columns[colIndex - _startCol] = (GetCellValue(cell, strings) ?? "").ToString();
                }
              }
            }
            else
            {
              values = new object[_colCount];
              foreach (var cell in row.OfType<Cell>())
              {
                colIndex = IndexFromColumnName(cell.CellReference.Value);
                if (colIndex >= _startCol && colIndex <= _endCol)
                {
                  values[colIndex - _startCol] = GetCellValue(cell, strings);
                }
              }
              yield return values;
            }
          }
        }
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    internal static DateTime ExtractDate(object value)
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
      if (string.IsNullOrEmpty(name)) return -1;
      int value = 0;
      foreach (var chr in name)
      {
        if (Char.IsLetter(chr))
        {
          if (value > 0) value *= 26;
          value += (int)Char.ToUpperInvariant(chr) - 64;
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

    /// <summary>
    /// Clean up resources
    /// </summary>
    public void Dispose()
    {
      if (_doc != null)
      {
        _doc.Close();
        _doc.Dispose();
        _doc = null;
      }
    }
  }
}
