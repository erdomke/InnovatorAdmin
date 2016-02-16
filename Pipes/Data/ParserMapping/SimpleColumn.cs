using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.ParserMapping
{
  public class SimpleColumn : IDestinationColumn
  {
    public string Label { get; set; }
    public string Name { get; set; }

    public override bool Equals(object obj)
    {
      var col = obj as SimpleColumn;
      if (col == null) return false;
      return this.Equals(col);
    }
    public bool Equals(SimpleColumn obj)
    {
      return obj.Label == this.Label && obj.Name == this.Name;
    }
    public override int GetHashCode()
    {
      return (this.Label ?? "").GetHashCode() ^ (this.Name ?? "").GetHashCode();
    }
  }
}
