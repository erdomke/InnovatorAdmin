using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Aras.Tools.InnovatorAdmin
{
  public class ExcelExtractor : IDataExtractor
  {
    private ExcelReader _reader;
    private bool _atEnd;
    private IEnumerator<object[]> _enum;
    private int _numProcessed = 0;
    private int _totalCount = -1;
    private string[] _columns = null;

    /// <summary>
    /// Whether the first row contains the column names
    /// </summary>
    public bool FirstRowColumnNames
    {
      get { return _reader.FirstRowColumnNames; }
      set { _reader.FirstRowColumnNames = value; }
    }
    /// <summary>
    /// The name of the sheet to pull data from
    /// </summary>
    public string Sheet
    {
      get { return _reader.Sheet; }
      set { _reader.Sheet = value; }
    }
    /// <summary>
    /// The names of all sheets in the spreadsheet
    /// </summary>
    public IEnumerable<string> SheetNames
    {
      get { return _reader.SheetNames; }
    }

    // For serialization
    internal ExcelExtractor() { }

    public ExcelExtractor(string path)
    {
      _reader = new ExcelReader();
      _reader.Initialize(new FileStream(path, FileMode.Open, FileAccess.Read));
    }
    public ExcelExtractor(Stream stream)
    {
      _reader = new ExcelReader();
      _reader.Initialize(stream);
    }

    public bool AtEnd
    {
      get { return _atEnd; }
    }
    public int GetTotalCount()
    {
      if (_totalCount >= 0) return _totalCount;
      return _reader.Count();
    }
    public int NumProcessed
    {
      get { return _numProcessed; }
    }
    public void Reset()
    {
      if (_enum != null) _enum.Dispose();
      _enum = _reader.GetEnumerator();
      _atEnd = false;
      _numProcessed = 0;
    }
    public void SetRange(int startCol, int startRow, int endCol, int endRow)
    {
      _reader.SetRange(startCol, startRow, endCol, endRow);
    }
    public void Write(int count, params IDataWriter[] writers)
    {
      if (_atEnd) return;
      if (_enum == null) Reset();

      int i = 0;
      while (i < count && AdvanceEnum())
      {
        foreach (var writer in writers)
        {
          writer.Row();
          for (var j = 0; j < _enum.Current.Length; j++)
          {
            writer.Cell(_columns[j], _enum.Current[j]);
          }
          writer.RowEnd();
        }
        i++;
      }
    }

    private bool AdvanceEnum()
    {
      _atEnd = !_enum.MoveNext();

      if (_atEnd)
      {
        return false;
      }
      else
      {
        if (_numProcessed == 0)
        {
          _columns = _reader.Columns;
          if (_columns == null)
          {
            _columns = new string[_reader.ColumnCount];
            for (var i = 0; i < _columns.Length; i++)
            {
              _columns[i] = "Column " + (i + 1);
            }
          }
          else
          {
            var grouped = new GroupedList<string, string>();
            int valueCount;
            for (var i = 0; i < _columns.Length; i++)
            {
              valueCount = grouped.Add(_columns[i].ToLowerInvariant().Replace(' ', '_'), _columns[i]);
              if (valueCount > 1) _columns[i] = _columns[i] + " " + valueCount.ToString();
            }
          }
        }
        _numProcessed++;
        return true;
      }
    }

  }
}
