using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace InnovatorAdmin.Editor
{
  public class SqlTokenizer : IEnumerable<SqlNode>
  {
    private string _text;
    private int _i = 0;
    private State _state;
    private SqlLiteral _current = new SqlLiteral();

    private enum State
    {
      Start,
      CommentMulti,
      CommentLine,
      StringLiteral,
      IdentifierBracket,
      IdentifierQuote,
      Identifier,
      NumericLiteral,
      BinaryLiteral,
      NumDecimalLiteral,
      NumExponentLiteral,
      Whitespace
    }

    public SqlTokenizer(string text)
    {
      _text = text;
    }

    public IEnumerator<SqlNode> GetEnumerator()
    {
      SqlNameDefinition name = null;

      foreach (var token in BasicEnumerator())
      {
        if (name != null)
        {
          if (name.TryAdd(token))
            continue;

          if (name.Any())
            yield return name;
          name = null;
        }

        if (KeywordPrecedesTable(token.Text))
        {
          name = new SqlNameDefinition();
        }
        yield return token;
      }
      if (name != null && name.Any())
        yield return name;
    }

    public static bool KeywordPrecedesTable(string value)
    {
      switch (value.ToLowerInvariant())
      {
        case "from":
        case "join":
        case "apply":
        case "update":
        case "into":
          return true;
      }
      return false;
    }

    public IEnumerable<SqlLiteral> BasicEnumerator()
    {
      _i = 0;
      while (_i < _text.Length)
      {
        switch (_state)
        {
          case State.Start:
            _current = new SqlLiteral();
            if (StartsWith('/', '*'))
            {
              _current.StartOffset = _i;
              _i++;
              _state = State.CommentMulti;
            }
            else if (StartsWith('-', '-'))
            {
              _current.StartOffset = _i;
              _i++;
              _state = State.CommentLine;
            }
            else if (StartsWith('N', '\''))
            {
              _current.StartOffset = _i;
              _i++;
              _state = State.StringLiteral;
            }
            else if (StartsWith('0', 'x'))
            {
              _current.StartOffset = _i;
              _i++;
              _state = State.BinaryLiteral;
            }
            else if (_text[_i] == '[')
            {
              _current.StartOffset = _i;
              _state = State.IdentifierBracket;
            }
            else if (_text[_i] == '"')
            {
              _current.StartOffset = _i;
              _state = State.IdentifierQuote;
            }
            else if (_text[_i] == '\'')
            {
              _current.StartOffset = _i;
              _state = State.StringLiteral;
            }
            else if (_text[_i] == '$')
            {
              _current.StartOffset = _i;
              _state = State.NumericLiteral;
            }
            else if (_text[_i] == '@')
            {
              _current.StartOffset = _i;
              _state = State.Identifier;
            }
            else if (char.IsDigit(_text[_i]))
            {
              _current.StartOffset = _i;
              _state = State.NumericLiteral;
            }
            else if (char.IsLetter(_text[_i]) || _text[_i] == '_')
            {
              _current.StartOffset = _i;
              _state = State.Identifier;
            }
            else if (char.IsWhiteSpace(_text[_i]))
            {
              _state = State.Whitespace;
            }
            else
            {
              yield return new SqlLiteral()
              {
                StartOffset = _i,
                Text = _text[_i].ToString(),
                Type = SqlType.Operator
              };
            }
            break;
          case State.BinaryLiteral:
            if (!IsHexDigit(_text[_i]))
            {
              _current.Text = _text.Substring(_current.StartOffset, _i - _current.StartOffset);
              _current.Type = SqlType.Binary;
              yield return _current;
              _i--;
              _state = State.Start;
            }
            break;
          case State.CommentLine:
            if (_text[_i] == '\r' || _text[_i] == '\n')
            {
              _current.Text = _text.Substring(_current.StartOffset, _i - _current.StartOffset);
              _current.Type = SqlType.Comment;
              yield return _current;
              _i--;
              _state = State.Start;
            }
            break;
          case State.CommentMulti:
            if (StartsWith('*', '/'))
            {
              _i++;
              _current.Text = _text.Substring(_current.StartOffset, _i + 1 - _current.StartOffset);
              _current.Type = SqlType.Comment;
              yield return _current;
              _state = State.Start;
            }
            break;
          case State.Identifier:
            if (!char.IsLetterOrDigit(_text[_i]) && _text[_i] != '_')
            {
              _current.Text = _text.Substring(_current.StartOffset, _i - _current.StartOffset);
              if ((_current.Text.Equals("group", StringComparison.OrdinalIgnoreCase)
                  || _current.Text.Equals("order", StringComparison.OrdinalIgnoreCase))
                && (StartsWith(' ', 'b', 'y')
                  || StartsWith(' ', 'B', 'Y')))
              {
                _i += 3;
                _current.Text = _text.Substring(_current.StartOffset, _i - _current.StartOffset);
              }
              _current.Type = SqlType.Identifier;
              yield return _current;
              _i--;
              _state = State.Start;
            }
            break;
          case State.IdentifierBracket:
            if (_text[_i] == ']')
            {
              _current.Text = _text.Substring(_current.StartOffset, _i + 1 - _current.StartOffset);
              _current.Type = SqlType.Identifier;
              yield return _current;
              _state = State.Start;
            }
            break;
          case State.IdentifierQuote:
            if (_text[_i] == '"')
            {
              _current.Text = _text.Substring(_current.StartOffset, _i + 1 - _current.StartOffset);
              _current.Type = SqlType.Identifier;
              yield return _current;
              _state = State.Start;
            }
            break;
          case State.NumericLiteral:
            if (_text[_i] == '.')
            {
              _state = State.NumDecimalLiteral;
            }
            else if (StartsWith('e', '+') || StartsWith('e', '-')
              || StartsWith('E', '+') || StartsWith('E', '-'))
            {
              _i++;
              _state = State.NumExponentLiteral;
            }
            else if (_text[_i] == 'e' || _text[_i] == 'E')
            {
              _i++;
              _state = State.NumExponentLiteral;
            }
            else if (!char.IsDigit(_text[_i]))
            {
              _current.Text = _text.Substring(_current.StartOffset, _i - _current.StartOffset);
              _current.Type = SqlType.Number;
              yield return _current;
              _i--;
              _state = State.Start;
            }
            break;
          case State.NumDecimalLiteral:
            if (StartsWith('e', '+') || StartsWith('e', '-')
              || StartsWith('E', '+') || StartsWith('E', '-'))
            {
              _i++;
              _state = State.NumExponentLiteral;
            }
            else if (_text[_i] == 'e' || _text[_i] == 'E')
            {
              _i++;
              _state = State.NumExponentLiteral;
            }
            else if (!char.IsDigit(_text[_i]))
            {
              _current.Text = _text.Substring(_current.StartOffset, _i - _current.StartOffset);
              _current.Type = SqlType.Number;
              yield return _current;
              _i--;
              _state = State.Start;
            }
            break;
          case State.NumExponentLiteral:
            if (!char.IsDigit(_text[_i]))
            {
              _current.Text = _text.Substring(_current.StartOffset, _i - _current.StartOffset);
              _current.Type = SqlType.Number;
              yield return _current;
              _i--;
              _state = State.Start;
            }
            break;
          case State.StringLiteral:
            if (StartsWith('\'', '\''))
            {
              _i++;
            }
            else if (_text[_i] == '\'')
            {
              _current.Text = _text.Substring(_current.StartOffset, _i + 1 - _current.StartOffset);
              _current.Type = SqlType.String;
              yield return _current;
              _state = State.Start;
            }
            break;
          case State.Whitespace:
            if (!char.IsWhiteSpace(_text[_i]))
            {
              _i--;
              _state = State.Start;
            }
            break;
        }
        _i++;
      }

      if (_current.StartOffset >= 0)
      {
        _current.Text = _text.Substring(_current.StartOffset);
        switch (_state)
        {
          case State.BinaryLiteral:
            _current.Type = SqlType.Binary;
            break;
          case State.Start:
          case State.Whitespace:
          case State.CommentLine:
          case State.CommentMulti:
            _current.Type = SqlType.Comment;
            break;
          case State.Identifier:
          case State.IdentifierBracket:
          case State.IdentifierQuote:
            _current.Type = SqlType.Identifier;
            break;
          case State.NumDecimalLiteral:
          case State.NumericLiteral:
          case State.NumExponentLiteral:
            _current.Type = SqlType.Number;
            break;
          case State.StringLiteral:
            _current.Type = SqlType.String;
            break;
        }
        yield return _current;
      }
    }

    internal static bool IsKeyword(string value)
    {
      switch (value.ToUpperInvariant())
      {
        case "ADD":
        case "ALL":
        case "ALTER":
        case "AND":
        case "ANY":
        case "AS":
        case "ASC":
        case "AUTHORIZATION":
        case "BACKUP":
        case "BEGIN":
        case "BETWEEN":
        case "BREAK":
        case "BROWSE":
        case "BULK":
        case "BY":
        case "CASCADE":
        case "CASE":
        case "CHECK":
        case "CHECKPOINT":
        case "CLOSE":
        case "CLUSTERED":
        case "COALESCE":
        case "COLLATE":
        case "COLUMN":
        case "COMMIT":
        case "COMPUTE":
        case "CONSTRAINT":
        case "CONTAINS":
        case "CONTAINSTABLE":
        case "CONTINUE":
        case "CONVERT":
        case "CREATE":
        case "CROSS":
        case "CURRENT":
        case "CURRENT_DATE":
        case "CURRENT_TIME":
        case "CURRENT_TIMESTAMP":
        case "CURRENT_USER":
        case "CURSOR":
        case "DATABASE":
        case "DBCC":
        case "DEALLOCATE":
        case "DECLARE":
        case "DEFAULT":
        case "DELETE":
        case "DENY":
        case "DESC":
        case "DISK":
        case "DISTINCT":
        case "DISTRIBUTED":
        case "DOUBLE":
        case "DROP":
        case "DUMP":
        case "ELSE":
        case "END":
        case "ERRLVL":
        case "ESCAPE":
        case "EXCEPT":
        case "EXEC":
        case "EXECUTE":
        case "EXISTS":
        case "EXIT":
        case "EXTERNAL":
        case "FETCH":
        case "FILE":
        case "FILLFACTOR":
        case "FOR":
        case "FOREIGN":
        case "FREETEXT":
        case "FREETEXTTABLE":
        case "FROM":
        case "FULL":
        case "FUNCTION":
        case "GOTO":
        case "GRANT":
        case "GROUP":
        case "HAVING":
        case "HOLDLOCK":
        case "IDENTITY":
        case "IDENTITYCOL":
        case "IDENTITY_INSERT":
        case "IF":
        case "IN":
        case "INDEX":
        case "INNER":
        case "INSERT":
        case "INTERSECT":
        case "INTO":
        case "IS":
        case "JOIN":
        case "KEY":
        case "KILL":
        case "LEFT":
        case "LIKE":
        case "LINENO":
        case "LOAD":
        case "MERGE":
        case "NATIONAL":
        case "NOCHECK":
        case "NONCLUSTERED":
        case "NOT":
        case "NULL":
        case "NULLIF":
        case "OF":
        case "OFF":
        case "OFFSETS":
        case "ON":
        case "OPEN":
        case "OPENDATASOURCE":
        case "OPENQUERY":
        case "OPENROWSET":
        case "OPENXML":
        case "OPTION":
        case "OR":
        case "ORDER":
        case "OUTER":
        case "OVER":
        case "PERCENT":
        case "PIVOT":
        case "PLAN":
        case "PRECISION":
        case "PRIMARY":
        case "PRINT":
        case "PROC":
        case "PROCEDURE":
        case "PUBLIC":
        case "RAISERROR":
        case "READ":
        case "READTEXT":
        case "RECONFIGURE":
        case "REFERENCES":
        case "REPLICATION":
        case "RESTORE":
        case "RESTRICT":
        case "RETURN":
        case "REVERT":
        case "REVOKE":
        case "RIGHT":
        case "ROLLBACK":
        case "ROWCOUNT":
        case "ROWGUIDCOL":
        case "RULE":
        case "SAVE":
        case "SCHEMA":
        case "SECURITYAUDIT":
        case "SELECT":
        case "SEMANTICKEYPHRASETABLE":
        case "SEMANTICSIMILARITYDETAILSTABLE":
        case "SEMANTICSIMILARITYTABLE":
        case "SESSION_USER":
        case "SET":
        case "SETUSER":
        case "SHUTDOWN":
        case "SOME":
        case "STATISTICS":
        case "SYSTEM_USER":
        case "TABLE":
        case "TABLESAMPLE":
        case "TEXTSIZE":
        case "THEN":
        case "TO":
        case "TOP":
        case "TRAN":
        case "TRANSACTION":
        case "TRIGGER":
        case "TRUNCATE":
        case "TRY_CONVERT":
        case "TSEQUAL":
        case "UNION":
        case "UNIQUE":
        case "UNPIVOT":
        case "UPDATE":
        case "UPDATETEXT":
        case "USE":
        case "USER":
        case "VALUES":
        case "VARYING":
        case "VIEW":
        case "WAITFOR":
        case "WHEN":
        case "WHERE":
        case "WHILE":
        case "WITH":
        case "WITHIN GROUP":
        case "WRITETEXT":
          return true;
      }
      return false;
    }

    private bool IsHexDigit(char value)
    {
      return char.IsDigit(value)
        || value == 'a' || value == 'b'
        || value == 'c' || value == 'd'
        || value == 'e' || value == 'f'
        || value == 'A' || value == 'B'
        || value == 'C' || value == 'D'
        || value == 'E' || value == 'F';
    }

    private bool StartsWith(params char[] ch)
    {
      if (_i + ch.Length > _text.Length) return false;
      for (var i = 0; i < ch.Length; i++)
      {
        if (_text[_i + i] != ch[i]) return false;
      }
      return true;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    public SqlGroup Parse()
    {
      var groups = new Stack<SqlGroup>();
      groups.Push(new SqlGroup());
      SqlLiteral literal;

      foreach (var node in this)
      {
        literal = node as SqlLiteral;
        if (groups.Peek().Any() && literal != null)
        {
          if (literal.Text == "("
            || literal.Text.Equals("case", StringComparison.OrdinalIgnoreCase)
            || literal.Text.Equals("begin", StringComparison.OrdinalIgnoreCase)
          )
          {
            groups.Push(new SqlGroup());
          }
        }
        groups.Peek().Add(node);
        if (literal != null)
        {
          if (literal.Text == ")"
            || literal.Text.Equals("end", StringComparison.OrdinalIgnoreCase))
          {
            var child = groups.Pop();
            if (!groups.Any()) groups.Push(new SqlGroup());
            groups.Peek().Add(child);
          }
          else if (literal.Text == ";")
          {
            var child = groups.Pop();
            if (!groups.Any())
            {
              groups.Push(new SqlGroup());
              groups.Peek().Add(child);
            }

            child = new SqlGroup();
            groups.Peek().Add(child);
            groups.Push(child);
          }
        }
      }

      var result = groups.Pop();
      if (result.Count < 1 && groups.Any())
        result = groups.Pop();
      if (result.Count == 1 && result[0] is SqlGroup)
        result = result[0] as SqlGroup;
      else if (result.Any() && result.Last() is SqlGroup && !((SqlGroup)result.Last()).Any())
      {
        result.Remove(result.Last());
      }

      while (groups.Any())
      {
        var newResult = groups.Pop();
        newResult.Add(result);
        result = newResult;
      }

      return result;
    }
  }
}
