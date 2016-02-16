using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public class Column : IColumn
  {
    private string _name;

    public ICellStyle Style { get; set; }
    public bool Visible { get; set; }
    public int Width { get; set; }
    public Type DataType { get; set; }
    public string Label { get; set; }
    public string Name { get { return _name; } }

    public Column(string name)
    {
      _name = name;
      this.Visible = true;
      this.Width = 50;
    }

    public override string ToString()
    {
      return this.Label ?? this.Name;
    }
  }
}
