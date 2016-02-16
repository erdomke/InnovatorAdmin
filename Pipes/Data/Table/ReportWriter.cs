using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pipes.Data.Table;

namespace Pipes.Data.Table
{
  public class ReportWriter : IProcessor<IReport>, IWriter<IFormattedTableWriter>
  {
    private IReport _data;
    private IFormattedTableWriter _writer;

    public object Parent { get; set; }

    public void InitializeData(IReport data)
    {
      _data = data;
    }

    public T Initialize<T>(T coreWriter) where T : IFormattedTableWriter
    {
      _writer = coreWriter;
      return coreWriter;
    }

    public void Execute()
    {
      try
      {
        _writer.Title(_data.Name);
        _writer.Part(ReportParts.Header);
        ProcessTable(_data.Header);
        _writer.PartEnd();
        _writer.Part(ReportParts.Data);
        ProcessTable(_data.Table);
        _writer.PartEnd();
        _writer.Part(ReportParts.Footer);
        ProcessTable(_data.Footer);
        _writer.PartEnd();
        _writer.Flush();
      }
      finally
      {
        var disposable = _writer as IDisposable;
        if (disposable != null) disposable.Dispose();
      }
    }

    private void ProcessTable(IFormattedTable table)
    {
      if (table != null)
      {
        _writer.Head();
        foreach (var column in table.Columns)
        {
          _writer.Column(column);
        }
        _writer.HeadEnd();
        foreach (var row in table.Rows)
        {
          _writer.Row();
          foreach (var cell in row.Cells)
          {
            _writer.Cell(cell);
          }
          _writer.RowEnd();
        }
      }
    }
  }
}
