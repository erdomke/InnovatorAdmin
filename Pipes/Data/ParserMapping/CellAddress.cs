using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.ParserMapping
{
  public class CellAddress
  {
    public string Column { get; set; }
    public int Row { get; set; }

    public override bool Equals(object obj)
    {
      var addr = obj as CellAddress;
      if (addr == null) return false;
      return this.Equals(addr);
    }
    public bool Equals(CellAddress obj)
    {
      return this.Column == obj.Column && this.Row == obj.Row;
    }
    public override int GetHashCode()
    {
      return (this.Column ?? "").GetHashCode() ^ this.Row.GetHashCode();
    }
  }
}
