using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public class SqlContext
  {
    private List<SqlTableInfo> _tables = new List<SqlTableInfo>();

    public IEnumerable<SqlTableInfo> Tables
    {
      get { return _tables; }
    }

    public bool TryByName(string name, out SqlTableInfo info)
    {
      info = ByName(name);
      return info != null;
    }

    public SqlTableInfo ByName(string name)
    {
      var processed = SqlName.ProcessName(name);
      return _tables.FirstOrDefault(t => string.Equals(t.Alias, processed, StringComparison.OrdinalIgnoreCase)
        || (t.Name != null && string.Equals(t.Name.Name, processed, StringComparison.OrdinalIgnoreCase))
        || (t.Name != null && string.Equals(t.Name.FullName, processed, StringComparison.OrdinalIgnoreCase)));
    }

    public SqlContext(SqlGroup node)
    {
      SqlGroup parentGroup = node.Parent as SqlGroup;
      while (!node.Any(n => SqlTokenizer.KeywordPrecedesTable(n as SqlLiteral)) && parentGroup != null)
      {
        node = parentGroup;
        parentGroup = node.Parent as SqlGroup;
      }

      var i = 0;
      SqlLiteral literal;
      SqlName name;
      SqlGroup group;
      while (i < node.Count)
      {
        literal = node[i] as SqlLiteral;
        if (SqlTokenizer.KeywordPrecedesTable(literal) && (i+1) < node.Count)
        {
          i++;
          name = node[i] as SqlName;
          group = node[i] as SqlGroup;
          if (name != null)
          {
            _tables.Add(new SqlTableInfo()
            {
              Name = name,
              Alias = name.Alias
            });
          }
          else if (group != null
            && group.Count > 5
            && group[0].TextEquals("(")
            && group[1].TextEquals("select"))
          {
            var info = new SqlTableInfo() {
              Columns = group.GetColumnNames()
            };
            if ((i + 1) < node.Count && node[i+1].Type == SqlType.Identifier)
            {
              i++;
              info.Alias = ((SqlLiteral)node[i]).Text;
            }
            _tables.Add(info);
          }
        }
        i++;
      }
    }
  }
}
