using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;

namespace Pipes.Data.ParserMapping
{
  public class TextFileDataSetParser : IDataSetParser
  {
    [DisplayName("Delimiter"), Description("Character used to separate fields on a single line of data.")]
    public char Delimiter { get; set; }
    [DisplayName("Enclosing Char Escape"), Description("Character used to escape the field enclosing character so that it may appear in a value.")]
    public char EnclosingCharEscape { get; set; }
    [DisplayName("Field Enclosing Char"), Description("Character used to enclose a field so the delimiter character may appear in a value.")]
    public char FieldEnclosingChar { get; set; }

    public TextFileDataSetParser()
    {
      this.Delimiter = ',';
      this.EnclosingCharEscape = '"';
      this.FieldEnclosingChar = '"';
    }

    public bool TryGetDataSet(System.IO.Stream stream, out DataSet ds)
    {
      ds = new DataSet();
      
      try
      {
        var delimReader = new IO.StreamTextSource(stream).Pipe(new Data.DelimitedTextLineReader());
        delimReader.FieldEnclosingChar = this.FieldEnclosingChar;
        delimReader.EnclosingCharEscape = this.EnclosingCharEscape;
        delimReader.AddDelim(this.Delimiter);
        var dataReader = delimReader.Pipe(new Data.DelimitedTextDataReader());
        dataReader.HasHeaders = false;

        var dt = ds.Tables.Add("Data");
        int colIndex = 0;
        DataRow newRow = null;

        foreach (var record in dataReader)
        {
          colIndex = 0;
          newRow = dt.NewRow();
          foreach (var field in record)
          {
            if (colIndex >= dt.Columns.Count)
              dt.Columns.Add("C" + (colIndex + 1).ToString("D2"), typeof(string));
            newRow[colIndex] = field.Value;
            colIndex += 1;
          }
          dt.Rows.Add(newRow);
        }
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
