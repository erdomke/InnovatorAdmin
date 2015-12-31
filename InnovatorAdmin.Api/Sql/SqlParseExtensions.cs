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

      SqlLiteral lastNode;
      for (var i = 1; i < segment.Length; i++)
      {
        if (segment[i].TextEquals(","))
        {
          lastNode = segment[i - 1].LastNonCommentNode() as SqlLiteral;
          if (lastNode != null && lastNode.Type == SqlType.Identifier)
            yield return lastNode.Text;
        }
      }

      lastNode = segment.Last().LastNonCommentNode() as SqlLiteral;
      if (lastNode != null && lastNode.Type == SqlType.Identifier)
        yield return lastNode.Text;
    }

    private static SqlNode LastNonCommentNode(this SqlNode node)
    {
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
