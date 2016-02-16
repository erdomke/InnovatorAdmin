using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public abstract class BaseFormattedTableWriter : IFormattedTableWriter
  {
    protected ReportParts _part = ReportParts.Data;

    public abstract IFormattedTableWriter Title(string name);
    public abstract IFormattedTableWriter Cell(ICell cell);
    public abstract IFormattedTableWriter Column(IColumn column);
    
    public abstract void Flush();

    public ITableWriter Cell(string name, object value)
    {
      this.Cell(new Cell(name, null) { Value = value });
      return this;
    }

    public ITableWriter Column(IColumnMetadata column)
    {
      this.Column(new Column(column.Name) { Label = column.Label, DataType = column.DataType });
      return this;
    }

    public abstract ITableWriter Head();
    public abstract ITableWriter HeadEnd();
    public abstract ITableWriter Row();
    public abstract ITableWriter RowEnd();

    public abstract void InitializeSettings(TableWriterSettings settings);

    public virtual IFormattedTableWriter Part(ReportParts part)
    {
      _part = part;
      return this;
    }

    public virtual IFormattedTableWriter PartEnd()
    {
      return this;
    }
  }
}
