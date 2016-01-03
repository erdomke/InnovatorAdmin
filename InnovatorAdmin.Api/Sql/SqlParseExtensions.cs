using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public static class SqlParseExtensions
  {
    public static bool TextEquals(this SqlNode node, string value)
    {
      var literal = node as SqlLiteral;
      if (literal != null && string.Equals(literal.Text, value, StringComparison.OrdinalIgnoreCase))
        return true;
      return false;
    }
    public static IEnumerable<string> GetColumnNames(this SqlGroup node)
    {
      var segment = node.SkipWhile(n => !n.TextEquals("select"))
        .Skip(1).TakeWhile(n => !n.TextEquals("from")).ToArray();
      if (!segment.Any())
        yield break;

      int start = 0;
      string name;
      for (var i = 1; i < segment.Length; i++)
      {
        if (segment[i].TextEquals(","))
        {
          name = ColumnName(segment, start, i);
          if (!string.IsNullOrWhiteSpace(name))
            yield return name;
          start = i + 1;
        }
      }

      name = ColumnName(segment, start, segment.Length);
      if (!string.IsNullOrWhiteSpace(name))
        yield return name;
    }

    private static string ColumnName(SqlNode[] segment, int start, int i)
    {
      if (i >= (start + 2) && segment[start + 1].TextEquals("=") && segment[start] is SqlLiteral)
      {
        var lastNode = segment[start] as SqlLiteral;
        if (lastNode != null && lastNode.Type == SqlType.Identifier)
          return lastNode.Text;
      }
      else
      {
        var lastNode = LastNonCommentNode(segment.Skip(start).Take(i - start)).LastNonCommentNode() as SqlLiteral;
        if (lastNode != null && lastNode.TextEquals("*"))
        {
          var idx = Array.IndexOf(segment, lastNode, start);
          if (idx > start && segment[idx - 1] is SqlName)
          {
            return (segment[idx - 1] as SqlName).FullName + "." + lastNode.Text;
          }
          else
          {
            return lastNode.Text;
          }
        }
        else if (lastNode != null && lastNode.Type == SqlType.Identifier)
        {
          return lastNode.Text;
        }
      }

      return null;
    }

    private static SqlNode LastNonCommentNode(IEnumerable<SqlNode> nodes)
    {
      return nodes.LastOrDefault(n => n.Type != SqlType.Comment);
    }
    private static SqlNode LastNonCommentNode(this SqlNode node)
    {
      if (node == null) return node;

      var last = node;
      var group = node as ISqlGroupNode;
      while (group != null)
      {
        last = group.Items.Where(i => i.Type != SqlType.Comment).Last();
        group = last as ISqlGroupNode;
      }

      if (last.Type == SqlType.Comment)
        return null;
      return last;
    }
  }
}
