using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Pipes.Json
{
  /// <summary>
  /// This class encodes and decodes JSON strings.
  /// Spec. details, see http://www.json.org/
  /// </summary>
  public class JsonTextReader : IPipeInput<System.IO.TextReader>, IPipeOutput<IJsonNode>
  {
    enum Token
    {
      None = -1,           // Used to denote no Lookahead available
      Curly_Open,
      Curly_Close,
      Squared_Open,
      Squared_Close,
      Colon,
      Comma,
      String,
      Number,
      True,
      False,
      Null
    }

    public void Initialize(IEnumerable<System.IO.TextReader> source)
    {
      _source = source;
    }

    public int MaxNestingDepth { get; set; }
    public bool StrictConformance { get; set; }

    public JsonTextReader()
    {
      this.StrictConformance = true;
      this.MaxNestingDepth = 19;
    }

    public IEnumerator<IJsonNode> GetEnumerator()
    {
      var stack = new Stack<JsonNodeType>();
      Token nextToken;
      Token currToken = Token.None;
      bool commaExpected = false;

      foreach (var source in _source)
      {
        buffer = new IO.BufferedCharArray(source);
        nextToken = LookAhead();
        while (nextToken != Token.None)
        {
          if (this.StrictConformance)
          {
            switch (currToken)
            {
              case Token.None:
                if (!(nextToken == Token.Curly_Open || nextToken == Token.Squared_Open)) throw new Exception("Valid json must start with an object or array");
                break;
              case Token.Comma:
                if (nextToken == Token.Comma || nextToken == Token.Curly_Close || nextToken == Token.Squared_Close) throw new Exception("A comma was found unexpectedly.");
                break;
              case Token.Curly_Open:
              case Token.Squared_Open:
                if (nextToken == Token.Comma) throw new Exception("A comma was found unexpectedly.");
                if (nextToken == Token.Colon) throw new Exception("A colon was found unexpectedly.");
                break;
              default:
                if (!(nextToken == Token.Comma || nextToken == Token.Colon || nextToken == Token.Curly_Close || nextToken == Token.Squared_Close)) throw new Exception("A comma, colon, or closing punctuation was expected but not found.");
                break;
            }
          }

          switch (nextToken)
          {
            case Token.Comma:
              ConsumeToken();
              commaExpected = false;
              break;
            case Token.Curly_Open:
              ConsumeToken();
              stack.Push(JsonNodeType.Object);
              yield return new JsonNode() { Type = JsonNodeType.Object };
              break;
            case Token.Curly_Close:
              ConsumeToken();
              if (stack.Pop() != JsonNodeType.Object) throw new Exception("Brace mismatch.");
              yield return new JsonNode() { Type = JsonNodeType.ObjectEnd };
              commaExpected = stack.Count > 0;
              break;
            case Token.Squared_Open:
              ConsumeToken();
              stack.Push(JsonNodeType.Array);
              yield return new JsonNode() { Type = JsonNodeType.Array };
              break;
            case Token.Squared_Close:
              ConsumeToken();
              if (stack.Pop() != JsonNodeType.Array) throw new Exception("Brace mismatch.");
              yield return new JsonNode() { Type = JsonNodeType.ArrayEnd };
              commaExpected = stack.Count > 0;
              break;
            default:
              if (stack.Count == 0)
              {
                throw new Exception("Data must be inside an array or object.");
              }
              else if (stack.Peek() == JsonNodeType.Object)
              {
                // name
                var result = new JsonNode();
                result.Name = ParseString();
                if (_ignorecase) result.Name = result.Name.ToLower();

                // :
                if (NextToken() != Token.Colon)
                {
                  throw new Exception("Expected colon");
                }

                nextToken = LookAhead();
                switch (nextToken)
                {
                  case Token.Curly_Open:
                    ConsumeToken();
                    stack.Push(JsonNodeType.Object);
                    result.Type = JsonNodeType.Object;
                    break;
                  case Token.Squared_Open:
                    ConsumeToken();
                    stack.Push(JsonNodeType.Array);
                    result.Type = JsonNodeType.Array;
                    break;
                  default:
                    result.Value = ParseValue();
                    result.Type = JsonNodeType.SimpleProperty;
                    commaExpected = true;
                    break;
                }
                yield return result;
              }
              else
              {
                yield return new JsonNode() { Type = JsonNodeType.Value, Value = ParseValue() };
                commaExpected = true;
              }
              break;
          }
          if (stack.Count > this.MaxNestingDepth) throw new Exception("Exceeds maximum nesting depth.");

          currToken = nextToken;
          nextToken = LookAhead();
        }

        if (stack.Count > 0) throw new Exception("One or more arrays or objects are not properly closed.");
        if (!(currToken == Token.Squared_Close || currToken == Token.Curly_Close)) throw new Exception("Extra tokens found after json object or array.");
      }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return this.GetEnumerator();
    }

    private IO.BufferedCharArray buffer;
    private IEnumerable<System.IO.TextReader> _source;
    readonly StringBuilder s = new StringBuilder();
    Token lookAheadToken = Token.None;
    bool _ignorecase = false;

    private Dictionary<string, object> ParseObject()
    {
      Dictionary<string, object> table = new Dictionary<string, object>();

      ConsumeToken(); // {

      while (true)
      {
        switch (LookAhead())
        {

          case Token.Comma:
            ConsumeToken();
            break;

          case Token.Curly_Close:
            ConsumeToken();
            return table;

          default:
            {

              // name
              string name = ParseString();
              if (_ignorecase)
                name = name.ToLower();

              // :
              if (NextToken() != Token.Colon)
              {
                throw new Exception("Expected colon");
              }

              // value
              object value = ParseValue();

              table[name] = value;
            }
            break;
        }
      }
    }

    private List<object> ParseArray()
    {
      List<object> array = new List<object>();
      ConsumeToken(); // [

      while (true)
      {
        switch (LookAhead())
        {

          case Token.Comma:
            ConsumeToken();
            break;

          case Token.Squared_Close:
            ConsumeToken();
            return array;

          default:
            array.Add(ParseValue());
            break;
        }
      }
    }

    private object ParseValue()
    {
      switch (LookAhead())
      {
        case Token.Number:
          return ParseNumber();

        case Token.String:
          return ParseString();

        case Token.Curly_Open:
          return ParseObject();

        case Token.Squared_Open:
          return ParseArray();

        case Token.True:
          ConsumeToken();
          return true;

        case Token.False:
          ConsumeToken();
          return false;

        case Token.Null:
          ConsumeToken();
          return null;
      }

      throw new Exception("Unrecognized token");
    }

    private string ParseString()
    {
      var inQuotes = true;
      ConsumeToken(); // "
      var prev = buffer.ReadFromLast();  // don't care about "
      if (!prev.EndsWith("\""))
      {
        buffer.MovePrev();
        inQuotes = false;
      }
      s.Length = 0;

      var c = buffer.MoveNext();

      while (c != '\0')
      {
        if (this.StrictConformance && (int)c < 32)
        {
          throw new Exception("Invalid string character");
        }
        else if (!inQuotes && !Char.IsLetterOrDigit(c) && c != '_')
        {
          buffer.MovePrev();
          s.Append(buffer.ReadFromLast());
          return s.ToString();
        } else if (c == '\\')
        {
          s.Append(buffer.ReadFromLast());
          s.Remove(s.Length - 1, 1);
          c = buffer.MoveNext();
          buffer.ReadFromLast();
          switch (c)
          {
            case '"':
            case '\\':
            case '/':
              s.Append(c);
              break;
            case 'b':
              s.Append('\b');
              break;
            case 'f':
              s.Append('\f');
              break;
            case 'n':
              s.Append('\n');
              break;
            case 'r':
              s.Append('\r');
              break;
            case 't':
              s.Append('\t');
              break;
            case 'u':
              {
                buffer.MoveNext(4);
                var code = buffer.ReadFromLast();
                if (code.Length < 4) throw new Exception("Invalid unicode code point.");
                  
                // parse the 32 bit hex into an integer codepoint
                uint codePoint = ParseUnicode(code[0], code[1], code[2], code[3]);
                s.Append((char)codePoint);
              }
              break;
            default:
              throw new Exception("Illegal escape sequence.");
          }
        }
        else if (c == '"')
        {
          s.Append(buffer.ReadFromLast());
          s.Remove(s.Length - 1, 1);
          return s.ToString();
        }
        c = buffer.MoveNext();
      }

      throw new Exception("Unexpectedly reached end of string");
    }

    private uint ParseSingleChar(char c1, uint multipliyer)
    {
      uint p1 = 0;
      if (c1 >= '0' && c1 <= '9')
        p1 = (uint)(c1 - '0') * multipliyer;
      else if (c1 >= 'A' && c1 <= 'F')
        p1 = (uint)((c1 - 'A') + 10) * multipliyer;
      else if (c1 >= 'a' && c1 <= 'f')
        p1 = (uint)((c1 - 'a') + 10) * multipliyer;
      return p1;
    }

    private uint ParseUnicode(char c1, char c2, char c3, char c4)
    {
      uint p1 = ParseSingleChar(c1, 0x1000);
      uint p2 = ParseSingleChar(c2, 0x100);
      uint p3 = ParseSingleChar(c3, 0x10);
      uint p4 = ParseSingleChar(c4, 1);

      return p1 + p2 + p3 + p4;
    }

    private long CreateLong(string s)
    {

      var i = 0;
      long num = 0;
      bool neg = false;

      if (string.IsNullOrEmpty(s)) throw new ArgumentNullException("s");
      if (s[i] == '-')
      {
        neg = true;
        i++;
      }
      else if (s[i] == '+')
      {
        neg = false;
        i++;
      }
      if (i >= s.Length) throw new FormatException("Invalid long format.");
      if (s[i] == '0' && s.Length > (i+1)) throw new Exception("Numbers cannot be zero-padded.");

      while (i < s.Length)
      {
        if (!Char.IsNumber(s[i])) throw new FormatException("Invalid long format.");
        num *= 10;
        num += (int)(s[i] - '0');
        i++;
      }

      return neg ? -num : num;
    }

    private object ParseNumber()
    {
      ConsumeToken();

      // Need to start back one place because the first digit is also a token and would have been consumed
      //var startIndex = index - 1;
      bool dec = false;
      var c = buffer.MoveNext();

      while ((c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+' || c == 'e' || c == 'E')
      {
        if (c == '.' || c == 'e' || c == 'E') dec = true;
        c = buffer.MoveNext();
      }
      buffer.MovePrev(); // Ignore the last non-number character.

      string s = buffer.ReadFromLast();
      if (dec) return double.Parse(s, NumberFormatInfo.InvariantInfo);
      return CreateLong(s);
    }

    private Token LookAhead()
    {
      if (lookAheadToken != Token.None) return lookAheadToken;

      return lookAheadToken = NextTokenCore();
    }

    private void ConsumeToken()
    {
      lookAheadToken = Token.None;
    }

    private Token NextToken()
    {
      var result = lookAheadToken != Token.None ? lookAheadToken : NextTokenCore();

      lookAheadToken = Token.None;

      return result;
    }

    private Token NextTokenCore()
    {
      buffer.ReadFromLast();
      char c = buffer.MoveNext();

      // Skip past whitespace
      while (c == ' ' || c == '\t' || c == '\n' || c == '\r')
      {
        buffer.ReadFromLast();
        c = buffer.MoveNext();
      }
 
      switch (c)
      {
        case '\0':
          return Token.None;

        case '{':
          return Token.Curly_Open;

        case '}':
          return Token.Curly_Close;

        case '[':
          return Token.Squared_Open;

        case ']':
          return Token.Squared_Close;

        case ',':
          return Token.Comma;

        case '"':
          return Token.String;

        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
        case '-':
        case '+':
        case '.':
          return Token.Number;

        case ':':
          return Token.Colon;

        case 'f':
          buffer.MoveNext(4);
          if (buffer.ReadFromLast() == "false")
          {
            return Token.False;
          }
          else
          {
            buffer.MovePrev(4);
          }
          break;
        case 't':
          buffer.MoveNext(3);
          if (buffer.ReadFromLast() == "true")
          {
            return Token.True;
          }
          else
          {
            buffer.MovePrev(3);
          }
          break;
        case 'n':
          buffer.MoveNext(3);
          if (buffer.ReadFromLast() == "null")
          {
            return Token.Null;
          }
          else
          {
            buffer.MovePrev(3);
          }
          break;
      }

      if (this.StrictConformance)
      {
        throw new Exception("Could not find token");
      }
      else
      {
        return Token.String;
      }
    }
  }
}
