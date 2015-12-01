using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aras.Tools.InnovatorAdmin.Editor
{
  public class SqlNameDefinition : SqlGroupBase<SqlLiteral>
  {
    public string Name { get; set; }
    public string Alias { get; set; }

    public bool TryAdd(SqlLiteral token)
    {
      if (token.Text.Equals("as", StringComparison.OrdinalIgnoreCase)
          && !this.Any(l => l.Text.Equals("as", StringComparison.OrdinalIgnoreCase)))
      {
        this.Add(token);
        return true;
      }
      else if (SqlTokenizer.IsKeyword(token.Text))
      {
        return false;
      }
      else if (!this.Any()
        || this.Last().Text == "." && token.Type == SqlType.Identifier)
      {
        this.Add(token);
        this.Name = ProcessName(token.Text);
        return true;
      }
      else if (this.Last().Text.Equals("as", StringComparison.OrdinalIgnoreCase)
        && token.Type == SqlType.Identifier)
      {
        this.Add(token);
        this.Alias = ProcessName(token.Text);
        return true;
      }
      else if (this.Last().Type == SqlType.Identifier && token.Text == ".")
      {
        this.Add(token);
        return true;
      }
      else if (SingleTrailingIdentifiers() && token.Type == SqlType.Identifier)
      {
        this.Add(token);
        this.Alias = ProcessName(token.Text);
        return true;
      }
      return false;
    }

    private string ProcessName(string value)
    {
      if (value[0] == '[' || value[0] == '"')
        return value.Substring(1, value.Length - 2);
      return value;
    }

    private bool SingleTrailingIdentifiers()
    {
      return this.Count > 0
        && this.Last().Type == SqlType.Identifier
        && (this.Count == 1 || this[this.Count - 2].Type != SqlType.Identifier);
    }
  }
}
