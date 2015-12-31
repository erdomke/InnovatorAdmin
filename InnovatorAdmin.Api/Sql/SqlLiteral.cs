using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class SqlLiteral : SqlNode
  {
    public string Text { get; set; }

    public override string ToString()
    {
      return this.Text;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode() ^ (this.Text ?? "").GetHashCode();
    }
    public override bool Equals(SqlNode obj)
    {
      var literal = obj as SqlLiteral;
      if (literal == null) return false;
      return base.Equals(obj) && this.Text == literal.Text;
    }

    public SqlLiteral PreviousLiteral()
    {
      var group = this.Parent as SqlGroup;

      while (group != null)
      {
        var idx = group.IndexOf(this);
        if (idx < 0)
          return null;
        else if (idx == 0)
          group = group.Parent as SqlGroup;
        else
          return group[idx - 1] as SqlLiteral;
      }
      return null;
    }
  }
}
