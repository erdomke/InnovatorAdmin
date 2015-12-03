using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InnovatorAdmin
{
  public class SqlParser
  {
    private enum SqlState
    {
      String,
      SingleLineComment,
      MultiLineComment,
      QuotedIdentifier,
      BracketIdentifier,
      Identifier,
      Operator,
      Other
    }

    private SqlState _state;
    private bool _keepCurrIdent;
    private string _lastIdentifier;
    private int _identStart;
    private List<string> _idents;

    public string SchemaToKeep { get; set; }

    public IEnumerable<string> FindSqlServerObjectNames(string sql)
    {
      _state = SqlState.Other;
      _keepCurrIdent = false;
      _lastIdentifier = string.Empty;
      _identStart = 0;
      _idents = new List<string>();

      for (int i = 0; i < sql.Length; i++)
      {
        switch (_state)
        {
          case SqlState.BracketIdentifier:
            if (sql[i] == ']')
            {
              PushIdent(sql.Substring(_identStart, i - _identStart));
              _state = SqlState.Other;
            }
            break;
          case SqlState.Identifier:
            if (!(char.IsLetterOrDigit(sql[i]) || sql[i] == '_'))
            {
              PushIdent(sql.Substring(_identStart, i - _identStart));
              if (sql[i] == '.' || sql[i] == '(')
              {
                _lastIdentifier += sql[i];
              }
              _state = SqlState.Other;
            }
            break;
          case SqlState.MultiLineComment:
            if (sql[i] == '/' && i > 0 && sql[i - 1] == '*') _state = SqlState.Other;
            break;
          case SqlState.QuotedIdentifier:
            if (sql[i] == '"')
            {
              PushIdent(sql.Substring(_identStart, i - _identStart));
              _state = SqlState.Other;
            }
            break;
          case SqlState.SingleLineComment:
            if (sql[i] == '\r' || sql[i] == '\n') _state = SqlState.Other;
            break;
          case SqlState.String:
            if (sql[i] == '\'') _state = SqlState.Other;
            break;
          case SqlState.Other:
            if (sql[i] == '[')
            {
              _identStart = i + 1;
              _state = SqlState.BracketIdentifier;
            }
            else if (sql[i] == '*' && i > 0 && sql[i - 1] == '/')
            {
              _state = SqlState.MultiLineComment;
            }
            else if (sql[i] == '"')
            {
              _identStart = i + 1;
              _state = SqlState.QuotedIdentifier;
            }
            else if (sql[i] == '-' && i > 0 && sql[i - 1] == '-')
            {
              _state = SqlState.SingleLineComment;
            }
            else if (sql[i] == '\'')
            {
              _state = SqlState.String;
            }
            else if (char.IsLetter(sql[i]))
            {
              _identStart = i;
              _state = SqlState.Identifier;
            }
            else if (sql[i] == '.' && i == _identStart + _lastIdentifier.Length)
            {
              _lastIdentifier += ".";
            }
            else if (char.IsWhiteSpace(sql[i]))
            {
              // Do nothing, for now
            }
            else
            {
              _lastIdentifier = string.Empty;
            }
            break;
        }
      }
      TryCaptureIdent();

      return _idents;
    }

    private void PushIdent(string ident)
    {
      if (_lastIdentifier.EndsWith("."))
      {
        _lastIdentifier += ident;
      }
      else
      {
        TryCaptureIdent();
        _keepCurrIdent = IsIdentTrigger(_lastIdentifier);
        _lastIdentifier = ident;
      }
    }
    private void TryCaptureIdent()
    {
      if (!string.IsNullOrEmpty(_lastIdentifier) 
        && (_keepCurrIdent || (!string.IsNullOrEmpty(SchemaToKeep) 
                              && _lastIdentifier.StartsWith(SchemaToKeep + ".", StringComparison.InvariantCultureIgnoreCase))))
        _idents.Add(_lastIdentifier.TrimEnd('('));
    }

    private static bool IsIdentTrigger(string ident)
    {
      switch (ident.ToLowerInvariant())
      {
        case "from":
        case "join":
        case "apply":
        case "into":
        case "update":
        case "procedure":
        case "view":
        case "trigger":
        case "table":
        case "function":
        case "index":
          return true;
        default:
          return false;
      }
    }
  }
}
