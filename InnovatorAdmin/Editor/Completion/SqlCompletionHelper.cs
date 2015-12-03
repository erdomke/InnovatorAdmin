using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class SqlCompletionHelper
  {
    private ISqlMetadataProvider _provider;

    public SqlCompletionHelper(ISqlMetadataProvider provider)
    {
      _provider = provider;
    }

    public IPromise<CompletionContext> Completions(string prefix, string all, int caret, string termCharacter)
    {
      try
      {
        var lastIndex = string.IsNullOrEmpty(termCharacter) ? -1 : all.IndexOf(termCharacter, caret);
        var sql = prefix + (lastIndex < 0 ? all.Substring(caret) : all.Substring(caret, lastIndex - caret));
        var parseTree = new SqlTokenizer(sql).Parse();
        if (!parseTree.Any())
          return Promises.Resolved(new CompletionContext());

        var currNode = parseTree.NodeByOffset(prefix.Length);
        var literal = currNode as SqlLiteral;

        if (literal != null)
        {

          if (literal.Text.Equals("from", StringComparison.OrdinalIgnoreCase)
            || literal.Text.Equals("join", StringComparison.OrdinalIgnoreCase)
            || literal.Text.Equals("apply", StringComparison.OrdinalIgnoreCase))
          {
            return Promises.Resolved(new CompletionContext()
            {
              Items = _provider.GetTableNames()
                .Concat(_provider.GetSchemaNames())
                .OrderBy(i => i),
              State = CompletionType.SqlObjectName,
              Overlap = 0
            });
          }
          else if (literal.Text == ".")
          {
            var name = literal.Parent as SqlNameDefinition;
            if (name != null)
            {
              var idx = name.IndexOf(literal);
              if (name[idx - 1].Text.Equals("innovator", StringComparison.OrdinalIgnoreCase))
              {
                return Promises.Resolved(new CompletionContext()
                {
                  Items = _provider.GetTableNames()
                    .Concat(_provider.GetSchemaNames())
                    .OrderBy(i => i),
                  State = CompletionType.SqlObjectName,
                  Overlap = 0
                });
              }
            }
            else
            {
              var group = literal.Parent as SqlGroup;
              if (group != null)
              {
                var idx = group.IndexOf(literal);
                var context = GetAliasContext(literal);
                string fullName;
                if (idx > 0 && group[idx - 1] is SqlLiteral
                  && context.TryGetValue(((SqlLiteral)group[idx - 1]).Text.ToLowerInvariant(), out fullName))
                {
                  return _provider.GetColumnNames(fullName)
                    .Convert(p => new CompletionContext()
                    {
                      Items = p,
                      State = CompletionType.SqlGeneral
                    });
                }
              }
            }
          }
          else if (SqlTokenizer.IsKeyword(literal.Text)
            || literal.Type == SqlType.Operator)
          {
            var group = literal.Parent as SqlGroup;
            if (group != null)
            {
              var types = group.OfType<SqlNameDefinition>().Select(n => _provider.GetColumnNames(n.Name)).ToArray();
              return Promises.All(types)
                .Convert(l => new CompletionContext()
                {
                  Items = l.OfType<IEnumerable<string>>()
                    .SelectMany(i => i)
                    .Distinct()
                    .OrderBy(i => i),
                  State = CompletionType.SqlGeneral
                });
            }
          }
        }

        //System.Diagnostics.Debug.Print(parseTree.NodeByOffset(prefix.Length).ToString());
        return Promises.Resolved(new CompletionContext());
      }
      catch (Exception ex)
      {
        return Promises.Rejected<CompletionContext>(ex);
      }
    }

    private IDictionary<string, string> GetAliasContext(SqlNode node)
    {
      var result = new Dictionary<string, string>();

      var group = node.Parent as SqlGroup;
      while (group != null)
      {
        foreach (var name in group.OfType<SqlNameDefinition>())
        {
          if (name.Alias != null && !result.ContainsKey(name.Alias.ToLowerInvariant()))
            result[name.Alias.ToLowerInvariant()] = name.Name;
          result[name.Name.ToLowerInvariant()] = name.Name;
        }
        group = group.Parent as SqlGroup;
      }

      return result;
    }

    
  }
}
