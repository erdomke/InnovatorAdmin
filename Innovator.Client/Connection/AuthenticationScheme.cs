using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  public class AuthenticationScheme
  {
    private string _name;
    private Dictionary<string, string> _params = new Dictionary<string, string>();

    public string Name { get { return _name; } }
    public IDictionary<string, string> Parameters { get { return _params; } }

    public AuthenticationScheme(string name)
    {
      _name = name.ToLowerInvariant();
    }

    private enum State
    {
      TokenStart,       //Expecting the start of another token
      Token,
      TokenEndWs,       //When a token ends with whitespace
      TokenEndWsSecond, //When a second consecutive token ends with whitespace
      ValueStart,       //Expecting the value for a parameter
      String
    }

    public static IEnumerable<AuthenticationScheme> Parse(string value)
    {
      if (string.IsNullOrEmpty(value)) return Enumerable.Empty<AuthenticationScheme>();

      value = value.Trim();
      var state = State.TokenStart;
      var pos = 0;
      var result = new List<AuthenticationScheme>();

      string lastToken = null;
      string lastParamName = null;
      var stringVal = new StringBuilder(value.Length);

      for (var i = 0; i < value.Length; i++)
      {
        switch (state)
        {
          case State.TokenStart:
            if (IsTokenChar(value[i]))
            {
              state = State.Token;
              pos = i;
            }
            else if (!char.IsWhiteSpace(value[i]) && value[i] != ',')
            {
              throw new FormatException("Invalid character detected while parsing the start of a token.");
            }
            break;
          case State.Token:
            if (char.IsWhiteSpace(value[i]))
            {
              if (string.IsNullOrEmpty(lastParamName))
              {
                if (lastToken == null)
                {
                  state = State.TokenEndWs;
                }
                else
                {
                  result.Add(new AuthenticationScheme(lastToken));
                  state = State.TokenEndWsSecond;
                }
                lastToken = value.Substring(pos, i - pos);
              }
              else if (result.Count > 0)
              {
                result.Last().Parameters.Add(lastParamName.ToLowerInvariant(), value.Substring(pos, i - pos));
                lastParamName = null;
                state = State.TokenStart;
              }
              else
              {
                throw new FormatException("Invalid parser state");
              }
            }
            else if (value[i] == ',')
            {
              if (string.IsNullOrEmpty(lastParamName))
              {
                throw new FormatException("Invalid parser state");
              }
              else if (result.Count > 0)
              {
                result.Last().Parameters.Add(lastParamName.ToLowerInvariant(), value.Substring(pos, i - pos));
                lastParamName = null;
              }
              else
              {
                throw new FormatException("Invalid parser state");
              }
              state = State.TokenStart;
            }
            else if (value[i] == '=')
            {
              if (lastToken == null)
              {
                if (result.Count < 1) throw new FormatException("Invalid parser state");
              }
              else
              {
                result.Add(new AuthenticationScheme(lastToken));
                lastToken = null;
              }
              lastParamName = value.Substring(pos, i - pos);
              state = State.ValueStart;
            }
            else if (!IsTokenChar(value[i]))
            {
              throw new FormatException("Invalid character detected while parsing a token.");
            }
            break;
          case State.TokenEndWs:
          case State.TokenEndWsSecond:
            if (value[i] == '=')
            {
              lastParamName = lastToken;
              lastToken = null;
              state = State.ValueStart;
            }
            else if (IsTokenChar(value[i]) && state != State.TokenEndWsSecond)
            {
              state = State.Token;
              pos = i;
            }
            else if (value[i] == ',' && state != State.TokenEndWsSecond)
            {
              result.Add(new AuthenticationScheme(lastToken));
              lastToken = null;
              state = State.TokenStart;
            }
            else if (!char.IsWhiteSpace(value[i]))
            {
              throw new FormatException("Invalid character detected while parsing the end of a token.");
            }
            break;
          case State.ValueStart:
            if (IsTokenChar(value[i]))
            {
              state = State.Token;
              pos = i;
            }
            else if (value[i] == '"')
            {
              stringVal.Length = 0;
              state = State.String;
            }
            else if (!char.IsWhiteSpace(value[i]))
            {
              throw new FormatException("Invalid character detected while parsing the start of a value.");
            }
            break;
          case State.String:
            if (value[i] == '"')
            {
              if (string.IsNullOrEmpty(lastParamName) || result.Count < 1)
              {
                throw new FormatException("Invalid parser state");
              }
              else
              {
                result.Last().Parameters.Add(lastParamName.ToLowerInvariant(), stringVal.ToString());
                lastParamName = null;
              }
              state = State.TokenStart;
            }
            else if (value[i] == '\\')
            {
              i++;
              stringVal.Append(value[i]);
            }
            else
            {
              stringVal.Append(value[i]);
            }
            break;
          default:
            throw new FormatException("Invalid parser state");
        }
      }

      switch (state)
      {
        case State.Token:
        case State.TokenEndWs:
          if (string.IsNullOrEmpty(lastParamName))
          {
            if (lastToken != null) throw new FormatException("Invalid parser state");
            result.Add(new AuthenticationScheme(value.Substring(pos, value.Length - pos)));
          }
          else if (result.Count > 0)
          {
            result.Last().Parameters.Add(lastParamName.ToLowerInvariant(), value.Substring(pos, value.Length - pos));
          }
          else
          {
            throw new FormatException("Invalid parser state");
          }
          break;
      }

      return result;
    }

    private static bool IsTokenChar(char value)
    {
      if (char.IsLetterOrDigit(value)) return true;
      switch (value)
      {
        case '!':
        case '#':
        case '$':
        case '%':
        case '&':
        case '\'':
        case '*':
        case '+':
        case '-':
        case '.':
        case '^':
        case '_':
        case '`':
        case '|':
        case '~':
          return true;
      }
      return false;
    }
  }
}
