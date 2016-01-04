using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class SqlDeclares
  {
    private HashSet<string> _names = new HashSet<string>();

    public IEnumerable<string> Names { get { return _names; } }

    public SqlDeclares(SqlGroup group)
    {
      var declares = group.OfType<SqlGroup>().Where(g => g.FirstOrDefault().TextEquals("declare"));
      foreach (var declare in declares)
      {
        ParseDeclare(declare);
      }
    }

    private void ParseDeclare(SqlGroup group)
    {
      var segment = group.ToArray();
      var start = 1;
      for (var i = 1; i < segment.Length; i++)
      {
        if (segment[i].TextEquals(","))
        {
          DeclareSegment(segment.Skip(start).Take(i - start));
          start = i + 1;
        }
      }
      DeclareSegment(segment.Skip(start).Take(segment.Length - start));
    }
    private void DeclareSegment(IEnumerable<SqlNode> nodes)
    {
      var literal = nodes.First() as SqlLiteral;
      if (literal != null && literal.Text.StartsWith("@"))
      {
        _names.Add(literal.Text);
      }
    }

  }
}
