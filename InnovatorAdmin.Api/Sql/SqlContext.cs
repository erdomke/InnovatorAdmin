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
    private List<string> _definitions = new List<string>();

    public IEnumerable<SqlTableInfo> Tables
    {
      get { return _tables; }
    }
    public IEnumerable<string> Definitions
    {
      get { return _definitions; }
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
      var parentGroup = node.Parent as SqlGroup;
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
        if (i == 0 && node[i].TextEquals("with"))
        {
          i++;

          while (i < node.Count && node[i] is SqlLiteral && !node[i].TextEquals("select"))
          {
            var info = new SqlTableInfo();
            info.Alias = ((SqlLiteral)node[i]).Text;
            _definitions.Add(info.Alias);
            i++;
            if (i >= node.Count) return;
            if (node[i].TextEquals("as")) i++;
            if (i >= node.Count) return;
            ProcessGroup(info, node[i] as SqlGroup);
            _tables.Add(info);
            if (node[i].TextEquals(",")) i++;
          }
        }
        else if (SqlTokenizer.KeywordPrecedesTable(literal) && (i+1) < node.Count)
        {
          i++;
          name = node[i] as SqlName;
          group = node[i] as SqlGroup;
          if (name != null)
          {
            if (!_tables.Any(t => string.Equals(t.Alias, name.ToString(), StringComparison.OrdinalIgnoreCase )))
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
            var info = new SqlTableInfo();
            ProcessGroup(info, group);

            if ((i + 1) < node.Count && node[i + 1].TextEquals("as"))
              i++;
            if ((i + 1) < node.Count && node[i + 1].Type == SqlType.Identifier)
            {
              i++;
              info.Alias = ((SqlLiteral)node[i]).Text;
            }
            _tables.Add(info);
          }
          else if (group != null
            && group.Count > 2
            && group[0] is SqlName
            && group[1].TextEquals("("))
          {
            var info = new SqlTableInfo() { Name = group[0] as SqlName };
            if ((i + 1) < node.Count && node[i + 1].TextEquals("as"))
              i++;
            if ((i + 1) < node.Count && node[i + 1].Type == SqlType.Identifier)
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

    private void ProcessGroup(SqlTableInfo info, SqlGroup group)
    {
      if (group == null) return;

      var cols = group.GetColumnNames().ToList();
      var toRemove = cols.Where(c => c.EndsWith("*")).ToArray();
      if (toRemove.Any(c => c == "*"))
      {
        var ctx = new SqlContext(group);
        cols.AddRange(ctx.Tables.Where(t => t.Columns != null).SelectMany(t => t.Columns));
        info.AdditionalColumns = ctx.Tables.Where(t => t.Columns == null && t.Name != null)
          .Concat(ctx.Tables.Where(t => t.AdditionalColumns != null).SelectMany(t => t.AdditionalColumns)).ToArray();
      }
      else if (cols.Any(c => c.EndsWith("*")))
      {
        var ctx = new SqlContext(group);
        var additional = new List<SqlTableInfo>();
        SqlTableInfo colInfo;
        foreach (var col in toRemove.Select(c => c.TrimEnd('.', '*')))
        {
          if (ctx.TryByName(col, out colInfo))
          {
            if (colInfo.Columns == null)
            {
              additional.Add(colInfo);
            }
            else
            {
              cols.AddRange(colInfo.Columns);
            }
          }
        }
        info.AdditionalColumns = additional;
      }

      foreach (var item in toRemove)
      {
        cols.Remove(item);
      }
      if (cols.Any())
        info.Columns = cols;
      return;
    }
  }
}
