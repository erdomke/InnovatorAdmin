using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public class Cell : ICell
  {
    private string _name;

    public object FormattedValue { get; set; }
    public ICellStyle Style { get; set; }
    public string Name { get { return _name; } }
    public object Value { get; set; }

    public Cell(IColumn parent) : this(parent, new CellStyle()) { }
    public Cell(IColumn parent, ICellStyle style) : this(parent.Name, style) { }
    public Cell(string parentName, ICellStyle style)
    {
      _name = parentName;
      this.Style = style;
    }
  }
}
