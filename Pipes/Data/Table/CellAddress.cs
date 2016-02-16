using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Pipes.Data.Table
{
  /// <summary>
  /// Structure containing the address of a spreadsheet cell
  /// </summary>
  public struct CellAddress
  {
    private int _column;
    private int _row;
    private int _sheet;

    /// <summary>
    /// The zero-based index of the column in the spreadsheet.
    /// </summary>
    [DisplayName("Column"), Description("The zero-based index of the column in the spreadsheet.")]
    public int Column { 
      get { return _column; }
      set { _column = value; }
    }

    /// <summary>
    /// Returns the A1 style column designator for the cell
    /// </summary>
    [DisplayName("Column Designator"), Description("Returns the A1 style column designator for the cell.")]
    public string ColumnDesignator
    {
      get { return GetColumnDesignator(this.Column); }
    }

    /// <summary>
    /// Returns the A1 style cell reference for the cell (e.g. AB14)
    /// </summary>
    [DisplayName("Cell Reference"), Description("Returns the A1 style column reference for the cell (e.g. AB14).")]
    public string CellReference
    {
      get { return GetColumnDesignator(this.Column) + (this.Row + 1); }
    }

    /// <summary>
    /// The zero-based index of the row in the spreadsheet.
    /// </summary>
    [DisplayName("Row"), Description("The zero-based index of the row in the spreadsheet.")]
    public int Row {
      get { return _row; }
      set { _row = value; }
    }

    /// <summary>
    /// The zero-based index of the sheet in the spreadsheet.
    /// </summary>
    [DisplayName("Sheet"), Description("The zero-based index of the sheet in the spreadsheet.")]
    public int Sheet {
      get { return _sheet; }
      set { _sheet = value; }
    }

    /// <summary>
    /// Create a new cell structure using an A1 or R1C1 style address
    /// </summary>
    /// <param name="address">Address of the cell</param>
    public CellAddress(string address)
    {
      if (string.IsNullOrEmpty(address)) throw new ArgumentException("Invalid address format");

      var i = 0;
      while (i < address.Length && char.IsLetter(address[i])) i++;
      if (i <= 0) throw new ArgumentException("Invalid address format");

      var initialText = address.Substring(0, i);
      while (i < address.Length && char.IsDigit(address[i])) i++;
      if (i <= initialText.Length) throw new ArgumentException("Invalid address format");

      _row = Convert.ToInt32(address.Substring(initialText.Length, i - initialText.Length)) - 1;

      if (i < (address.Length - 1) && 
        (initialText == "r" || initialText == "R") && 
        (address[i] == 'c' || address[i] == 'C')) 
      {
        i++;
        var start = i;
        while (i < address.Length && char.IsDigit(address[i])) i++;
        if (i <= start) throw new ArgumentException("Invalid address format");
        _column = Convert.ToInt32(address.Substring(start, i - start)) - 1;
      }
      else
      {
        _column = GetColumnIndex(initialText);
      }
      _sheet = 0;
    }

    /// <summary>
    /// Create a new cell structure using the zero-based row and column indices of the cell
    /// </summary>
    /// <param name="row">Zero-based row index</param>
    /// <param name="column">Zero-based column index</param>
    public CellAddress(int row, int column)
      : this(0, row, column)
    {
    }
    /// <summary>
    /// Create a new cell structure using the zero-based sheet, row, and column indices of the cell
    /// </summary>
    /// <param name="sheet">Zero-based sheet index</param>
    /// <param name="row">Zero-based row index</param>
    /// <param name="column">Zero-based column index</param>
    public CellAddress(int sheet, int row, int column)
    {
      _sheet = sheet;
      _row = row;
      _column = column;
    }

    public override bool Equals(object obj)
    {
      if (obj is CellAddress)
      {
        return Equals((CellAddress)obj);
      }
      else
      {
        return false;
      }
    }
    public bool Equals(CellAddress obj)
    {
      return obj.Sheet == this.Sheet && obj.Row == this.Row && obj.Column == this.Column;
    }
    public override int GetHashCode()
    {
      return this.Sheet ^ this.Row ^ this.Column;
    }
    /// <summary>
    /// Returns an address for a cell that is <paramref name="rows"/> down and <paramref name="columns"/> to the right of the current cell.
    /// </summary>
    /// <param name="rows">Number of rows (positive or negative) down from the current cell</param>
    /// <param name="columns">Number of columns (positive or negative) to the right of the current cell</param>
    /// <returns>Address of the offset cell</returns>
    public CellAddress Offset(int rows, int columns)
    {
      if ((this.Row + rows) < 0 || (this.Column + columns) < 0)
      {
        throw new ArgumentException("The supplied offset does not represent a valid cell.");
      }
      else
      {
        return new CellAddress(this.Sheet, this.Row + rows, this.Column + columns);
      }
    }
    public override string ToString()
    {
      return string.Format("Sheet{0}!{1}{2}", this.Sheet + 1, GetColumnDesignator(this.Column), this.Row + 1);
    }

    /// <summary>
    /// Return the numeric index of a column based on a string name
    /// </summary>
    /// <param name="columnName">String name of the column (e.g. XFD)</param>
    /// <returns>Zero-based numeric index of the column</returns>
    public static int GetColumnIndex(string columnName)
    {
      int returnVal = 0;

      for (int i = 0; i <= columnName.Length - 1; i++)
      {
        returnVal = returnVal * 26 + GetCharacterValue(columnName[i]);
      }

      return returnVal - 1;
    }
    /// <summary>
    /// Returns the A1-style column designator for a zero-based column index
    /// </summary>
    /// <param name="index">Zero-based column index</param>
    /// <returns>A1-style column designator</returns>
    public static string GetColumnDesignator(int index)
    {
      StringBuilder builder = new StringBuilder(6);
      index += 1;

      while (index > 26)
      {
        if (index % 26 > 0)
        {
          builder.Insert(0, GetCharacter(index % 26));
          index /= 26;
        }
        else
        {
          builder.Insert(0, GetCharacter(26));
          index /= 26;
          index -= 1;
        }
      }
      builder.Insert(0, GetCharacter(index));
      return builder.ToString();
    }
    /// <summary>
    /// Returns the character for a given index.  1 = "A", 26 = "Z"
    /// </summary>
    /// <param name="index">Index of the character</param>
    /// <returns>A-Z character</returns>
    private static char GetCharacter(int index)
    {
      return (char)(index + 64);
    }
    /// <summary>
    /// Returns the numeric value of a character where "A" is 1 (regardless of case).
    /// </summary>
    /// <param name="character">Character to test</param>
    /// <returns>A numeric value between 1 and 26 for inputs between "A" and "Z" or "a" and "z".  Throws an exception otherwise.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="character"/> &lt; "a"/"A" or <paramref name="character"/> &gt; "z"/"Z"</exception>
    private static int GetCharacterValue(char character)
    {
      var ch = char.ToLower(character);
      if (ch >= 'a' && ch <= 'z')
      {
        return (int)(ch) - 96;
      }
      else 
      {
        throw new ArgumentOutOfRangeException("character", "Must be in the range of 'a' to 'z', upper or lower case.");
      }      
    }

  }

}
