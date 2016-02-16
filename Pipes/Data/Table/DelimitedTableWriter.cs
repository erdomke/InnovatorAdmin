using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace Pipes.Data.Table
{
  public abstract class DelimitedTableWriter : Table.BaseFormattedTableWriter, IWriter<TextWriter>
  {
    private TextWriter _writer;
    private TableWriterSettings _settings;
    private Dictionary<string, Table.IColumn> _columns = new Dictionary<string,Table.IColumn>();
    private bool _first;

    public object Parent { get; set; }

    /// <summary>
    /// The delimiter which separates fields of data within a record
    /// </summary>
    [DisplayName("Delimiter"), Description("The delimiter which separates fields of data within a record.")]
    protected abstract string Delimiter { get; }

    /// <summary>
    /// Whether or not field values are enclosed in quotes
    /// </summary>
    [DisplayName("Enclose Field in Quotes"), Description("Whether or not field values are enclosed in quotes.")]
    protected abstract bool EncloseFieldsInQuotes { get; }

    public override Table.IFormattedTableWriter Cell(Table.ICell cell)
    {
      if (_part == ReportParts.Data)
      {
        Table.IColumn column = null;
        if (!_columns.TryGetValue(cell.Name, out column) || column.Visible || _settings.HiddenColumnHandling == HiddenColumnOptions.IncludeVisible)
        {
          WriteValue(cell.FormattedValue ?? cell.Value, (cell.Style == null ? 0 : cell.Style.Indent));
        }
      }
      return this;
    }

    public override Table.IFormattedTableWriter Column(Table.IColumn column)
    {
      if (_part == ReportParts.Data)
      {
        _columns[column.Name] = column;
        if (_settings.IncludeHeaders && (_settings.HiddenColumnHandling == HiddenColumnOptions.IncludeVisible || column.Visible))
        {
          WriteValue(column.Label ?? column.Name, (column.Style == null ? 0 : column.Style.Indent));
        }
      }
      return this;
    }

    public override void InitializeSettings(TableWriterSettings settings)
    {
      _settings = settings;
    }

    public override void Flush()
    {
      _writer.Flush();
    }

    public override Table.ITableWriter Head()
    {
      if (_part == ReportParts.Data) _first = true;
      return this;
    }

    public override Table.ITableWriter HeadEnd()
    {
      if (_part == ReportParts.Data && _settings.IncludeHeaders) _writer.WriteLine();
      return this;
    }

    public override Table.ITableWriter Row()
    {
      if (_part == ReportParts.Data) _first = true;
      return this;
    }

    public override Table.ITableWriter RowEnd()
    {
      if (_part == ReportParts.Data) _writer.WriteLine();
      return this;
    }

    public T Initialize<T>(T coreWriter) where T : TextWriter
    {
      _writer = coreWriter;
      return coreWriter;
    }

    private void WriteValue(object value, int indent)
    {
      if (!_first) _writer.Write(Delimiter);
      if (EncloseFieldsInQuotes) _writer.Write("\"");

      // render the indent
      for (int i = 1; i <= indent; i++)
      {
        _writer.Write("  ");
      }

      // Render the value
      if (value == null)
      {
        _writer.Write(string.Empty);
      }
      else if (EncloseFieldsInQuotes)
      {
        _writer.Write(value.ToString().Replace("\"", "\"\""));
      }
      else
      {
        _writer.Write(value.ToString().Replace(Delimiter, Delimiter + Delimiter));
      }

      if (EncloseFieldsInQuotes) _writer.Write("\"");
      _first = false;
    }

    public override Table.IFormattedTableWriter Title(string name)
    {
      return this;
    }
  }
}
