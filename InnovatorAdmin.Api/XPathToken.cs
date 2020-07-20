using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public enum XPathTokenType
  {
    Separator,
    Operator,
    String,
    Number,
    AxisName,
    NameTest,
    NodeType,
    FunctionName,
    VariableReference
  }

  public enum XPathAxis
  {
    Self,
    Ancestor,
    AncestorOrSelf,
    Attribute,
    Child,
    Descendant,
    DescendantOrSelf,
    Following,
    FollowingSibling,
    Namespace,
    Parent,
    Preceding,
    PrecedingSibling,
  }

  [DebuggerDisplay("{Value,nq} ({Type})")]
  public class XPathToken
  {
    public XPathTokenType Type { get; }
    public string Value { get; private set; }

    public bool TryGetAxis(out XPathAxis axis)
    {
      axis = XPathAxis.Self;
      if (Type == XPathTokenType.AxisName)
      {
        if (Value == "@")
        {
          axis = XPathAxis.Attribute;
          return true;
        }

        if (Enum.TryParse(Value.Replace("-", ""), true, out axis))
          return true;
      }
      return false;
    }

    public bool TryGetNameTest(out string name)
    {
      name = null;
      if (Type == XPathTokenType.NameTest)
      {
        name = Value;
        return true;
      }
      return false;
    }

    public bool TryGetString(out string value)
    {
      value = null;
      if (Type == XPathTokenType.String)
      {
        var quote = Value[0];
        value = Value.Substring(1, Value.Length - 2).Replace(new string(quote, 2), new string(quote, 1));
        return true;
      }
      return false;
    }

    private XPathToken(XPathTokenType type, string value)
    {
      Type = type;
      Value = value;
    }

    public static IEnumerable<XPathToken> Parse(string xPath)
    {
      if (string.IsNullOrEmpty(xPath))
        return Enumerable.Empty<XPathToken>();

      var result = new List<XPathToken>();

      int index = 0;
      while (index < xPath.Length)
      {
        switch (xPath[index])
        {
          case '(':
          case ')':
          case '[':
          case ']':
          case ',':
            result.Add(new XPathToken(XPathTokenType.Separator, xPath.Substring(index, 1)));
            index++;
            break;
          case '|':
          case '=':
            result.Add(new XPathToken(XPathTokenType.Operator, xPath.Substring(index, 1)));
            index++;
            break;
          case '+':
          case '-':
            if (TryGetNumber(xPath, index, out var signedNumber))
            {
              result.Add(new XPathToken(XPathTokenType.Number, signedNumber));
              index += signedNumber.Length;
            }
            else
            {
              result.Add(new XPathToken(XPathTokenType.Operator, xPath.Substring(index, 1)));
              index++;
            }
            break;
          case '@':
            result.Add(new XPathToken(XPathTokenType.AxisName, xPath.Substring(index, 1)));
            index++;
            break;
          case ':':
            throw new NotSupportedException();
          case '$':
            if (index + 1 < xPath.Length && TryGetName(xPath, index + 1, out var variableName))
            {
              result.Add(new XPathToken(XPathTokenType.VariableReference, xPath.Substring(index, variableName.Length + 1)));
              index += 1 + variableName.Length;
            }
            else
            {
              throw new InvalidOperationException();
            }
            break;
          case '.':
            if (TryGetNumber(xPath, index, out var decimalNumber))
            {
              result.Add(new XPathToken(XPathTokenType.Number, decimalNumber));
              index += decimalNumber.Length;
            }
            else if (index + 1 < xPath.Length && xPath[index + 1] == '.')
            {
              result.Add(new XPathToken(XPathTokenType.Separator, xPath.Substring(index, 2)));
              index += 2;
            }
            else
            {
              result.Add(new XPathToken(XPathTokenType.Separator, xPath.Substring(index, 1)));
              index++;
            }
            break;
          case '*':
            var asteriskType = (result.Count > 0 && !(result.Last().Type == XPathTokenType.AxisName || result.Last().Type == XPathTokenType.Operator
              || result.Last().Value == "(" || result.Last().Value == "[")) ? XPathTokenType.Operator : XPathTokenType.NameTest;
            result.Add(new XPathToken(asteriskType, xPath.Substring(index, 1)));
            index++;
            break;
          case '!':
            if (index + 1 < xPath.Length && xPath[index + 1] == '=')
            {
              result.Add(new XPathToken(XPathTokenType.Operator, xPath.Substring(index, 2)));
              index += 2;
            }
            else
            {
              throw new InvalidOperationException($"`{xPath[index]}` is an invalid xPath token");
            }
            break;
          case '<':
          case '>':
            if (index + 1 < xPath.Length && xPath[index + 1] == '=')
            {
              result.Add(new XPathToken(XPathTokenType.Operator, xPath.Substring(index, 2)));
              index += 2;
            }
            else
            {
              result.Add(new XPathToken(XPathTokenType.Operator, xPath.Substring(index, 1)));
              index++;
            }
            break;
          case '/':
            if (index + 1 < xPath.Length && xPath[index + 1] == '/')
            {
              result.Add(new XPathToken(XPathTokenType.Operator, xPath.Substring(index, 2)));
              index += 2;
            }
            else
            {
              result.Add(new XPathToken(XPathTokenType.Operator, xPath.Substring(index, 1)));
              index++;
            }
            break;
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
            if (TryGetNumber(xPath, index, out var digits))
            {
              result.Add(new XPathToken(XPathTokenType.Number, digits));
              index += digits.Length;
            }
            else
            {
              throw new InvalidOperationException();
            }
            break;
          case '\'':
          case '"':
            if (TryGetString(xPath, index, out var stringValue))
            {
              result.Add(new XPathToken(XPathTokenType.String, stringValue));
              index += stringValue.Length;
            }
            else
            {
              throw new InvalidOperationException();
            }
            break;
          default:
            if (TryGetName(xPath, index, out var name))
            {
              int nextChar;
              switch (name)
              {
                case "ancestor":
                case "ancestor-or-self":
                case "attribute":
                case "child":
                case "descendant":
                case "descendant-or-self":
                case "following":
                case "following-sibling":
                case "namespace":
                case "parent":
                case "preceding":
                case "preceding-sibling":
                case "self":
                  nextChar = SkipWhitespace(xPath, index + name.Length);
                  if (nextChar + 1 < xPath.Length && xPath.Substring(nextChar, 2) == "::")
                  {
                    result.Add(new XPathToken(XPathTokenType.AxisName, name + "::"));
                    index = SkipWhitespace(xPath, nextChar + 2);
                    continue;
                  }
                  break;
                case "comment":
                case "text":
                case "processing-instruction":
                case "node":
                  nextChar = SkipWhitespace(xPath, index + name.Length);
                  if (nextChar < xPath.Length && xPath[nextChar] == '(')
                  {
                    result.Add(new XPathToken(XPathTokenType.NodeType, name));
                    index = nextChar;
                    continue;
                  }
                  break;
                case "and":
                case "or":
                case "mod":
                case "div":
                  if (result.Count > 0 && !(result.Last().Type == XPathTokenType.AxisName || result.Last().Type == XPathTokenType.Operator
                    || result.Last().Value == "(" || result.Last().Value == "["))
                  {
                    result.Add(new XPathToken(XPathTokenType.Operator, name));
                    index = SkipWhitespace(xPath, index + name.Length);
                    continue;
                  }
                  break;
              }

              nextChar = SkipWhitespace(xPath, index + name.Length);
              if (nextChar < xPath.Length && xPath[nextChar] == '(')
              {
                result.Add(new XPathToken(XPathTokenType.FunctionName, name));
              }
              else if (index + name.Length + 1 < xPath.Length && xPath[index + name.Length] == ':')
              {
                if (xPath[index + name.Length + 1] == '*')
                {
                  result.Add(new XPathToken(XPathTokenType.NameTest, name + ":*"));
                  nextChar = index + name.Length + 2;
                }
                else if (TryGetName(xPath, index + name.Length + 1, out var localName))
                {
                  result.Add(new XPathToken(XPathTokenType.NameTest, name + ":" + localName));
                  nextChar = index + name.Length + localName.Length + 1;
                }
                else
                {
                  throw new InvalidOperationException();
                }
              }
              else
              {
                result.Add(new XPathToken(XPathTokenType.NameTest, name));
              }
              index = nextChar;
            }
            else
            {
              throw new InvalidOperationException();
            }
            break;
        }
        index = SkipWhitespace(xPath, index);
      }

      return result;
    }

    private static int SkipWhitespace(string input, int index)
    {
      while (index < input.Length && char.IsWhiteSpace(input[index]))
        index++;
      return index;
    }

    private static bool IsNcNameStartChar(char ch)
    {
      return (ch >= 'A' && ch <= 'Z')
        || ch == '_'
        || (ch >= 'a' && ch <= 'z')
        || (ch >= 0xC0 && ch <= 0xD6)
        || (ch >= 0xD8 && ch <= 0xF6)
        || (ch >= 0xF8 && ch <= 0x2FF)
        || (ch >= 0x370 && ch <= 0x37D)
        || (ch >= 0x37F && ch <= 0x1FFF)
        || (ch >= 0x200C && ch <= 0x200D)
        || (ch >= 0x2070 && ch <= 0x218F)
        || (ch >= 0x2C00 && ch <= 0x2FEF)
        || (ch >= 0x3001 && ch <= 0xD7FF)
        || (ch >= 0xF900 && ch <= 0xFDCF)
        || (ch >= 0xFDF0 && ch <= 0xFFFD)
        || (ch >= 0x10000 && ch <= 0xEFFFF);
    }

    private static bool IsNcNameChar(char ch)
    {
      return IsNcNameStartChar(ch)
        || ch == '-'
        || ch == '.'
        || (ch >= '0' && ch <= '9')
        || ch == 0xB7
        || (ch >= 0x0300 && ch <= 0x036F)
        || (ch >= 0x203F && ch <= 0x2040);
    }

    private static bool TryGetString(string input, int start, out string stringValue)
    {
      stringValue = string.Empty;
      if (start >= input.Length || (input[start] != '\'' && input[start] != '"'))
        return false;

      var quoteChar = input[start];
      var curr = start + 1;
      while (curr < input.Length)
      {
        if (input[curr] == quoteChar && !(curr + 1 < input.Length && input[curr + 1] == quoteChar))
        {
          stringValue = input.Substring(start, curr - start + 1);
          return true;
        }
        curr++;
      }
      return false;
    }

    public static bool IsNcName(string value)
    {
      return TryGetName(value, 0, out var name) && name.Length == value.Length;
    }

    private static bool TryGetName(string input, int start, out string name)
    {
      name = string.Empty;
      if (start >= input.Length || !IsNcNameStartChar(input[start]))
        return false;
      var curr = start + 1;
      while (curr < input.Length && IsNcNameChar(input[curr]))
        curr++;
      name = input.Substring(start, curr - start);
      return true;
    }

    private static bool TryGetNumber(string input, int start, out string number)
    {
      number = string.Empty;
      if (start >= input.Length)
        return false;

      var hasDigit = false;
      var curr = start;
      if (input[curr] == '+' || input[curr] == '-')
        curr++;
      while (curr < input.Length && input[curr] >= '0' && input[curr] <= '9')
      {
        hasDigit = true;
        curr++;
      }
      if (curr < input.Length && input[curr] == '.')
        curr++;
      while (curr < input.Length && input[curr] >= '0' && input[curr] <= '9')
      {
        hasDigit = true;
        curr++;
      }

      if (curr == start || !hasDigit)
        return false;

      if (curr < input.Length && (input[curr] == 'e' || input[curr] == 'E'))
      {
        curr++;
        if (curr < input.Length && input[curr] == '+' || input[curr] == '-')
          curr++;
        while (curr < input.Length && input[curr] >= '0' && input[curr] <= '9')
          curr++;
      }

      number = input.Substring(start, curr - start);
      return true;
    }
  }
}
