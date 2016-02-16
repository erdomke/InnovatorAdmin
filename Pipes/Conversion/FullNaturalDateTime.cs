using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Pipes.Conversion
{
  public class FullNaturalDateTime
  {
    public enum TokenType
    {
      Unknown,
      Text,
      Digit,
      Punctuation,

      Number,
      Month,
      Weekday,
      Meridian,
      OrdinalSuffix,
      Ordinal
    }

    public class Token
    {
      private string _text;

      public TokenType Type { get; set; }
      public string Text 
      {
        get { return _text; }
        set { _text = value.ToLowerInvariant(); }
      }
      public int Value { get; set; }

      public Token() { }
      public Token(TokenType type)
      {
        this.Type = type;
      }

      public override string ToString()
      {
        return this.Type.ToString() + ": " + _text;
      }
    }

    public static ApproxDateTime[] Parse(string value)
    {
      return Parse(value, new DateTime(2011, 2, 28), CultureInfo.CurrentCulture);
    }

    public static ApproxDateTime[] Parse(string value, DateTime reference, CultureInfo culture)
    {
      EnsureLookups(culture);

      string state = "";
      var stack = new List<Token>();
      ApproxDateTime newDate;
      var dates = new List<ApproxDateTime>();
      bool valid;

      foreach (var token in GetInterpretedTokens(value))
      {
        valid = false;
        switch (token.Type)
        {
          case TokenType.Punctuation:
            state += token.Text;
            valid = true;
            break;
          case TokenType.Digit:
            switch (state)
            {
              case "":
              case "D-":
              case "D-D-":
              case "D/":
              case "D/D/":
              case "D:":
              case "D:D:":
              case "D:D:D.":
              case "D":
              case "DD":
              case "D.":
              case "D.D.":
              case "M":
              case "MD,":
              case "MO,":
              case "OM":
              case "DM":
                state += "D";
                valid = true;
                break;
              default:
                if (state.EndsWith("'"))
                {
                  state += "D";
                  valid = true;
                }
                break;
            }
            break;
          case TokenType.Month:
            if (state == "" || state.EndsWith("D") || state.EndsWith("O"))
            {
              state += "M";
              valid = true;
            }
            break;
          case TokenType.Ordinal:
            if (state == "" || state.EndsWith("M"))
            {
              state += "O";
              valid = true;
            }
            break;
          case TokenType.Meridian:
            if (state.EndsWith("D"))
            {
              state += "P";
              valid = true;
            }
            break;
          case TokenType.Text:
            switch (token.Text)
            {
              case "year":
                state += token.Text;
                break;
            }
            break;
        }

        if (!valid && state != "")
        {
          newDate = HandleStack(state, stack, reference, culture);
          if (newDate != null) AddApproxDateTime(dates, newDate);
          state = "";
          stack.Clear();

          switch (token.Type)
          {
            case TokenType.Digit:
              state = "D";
              break;
            case TokenType.Month:
              state = "M";
              break;
            case TokenType.Ordinal:
              state = "O";
              break;
          }
        }

        if (state != "") stack.Add(token);
      }

      newDate = HandleStack(state, stack, reference, culture);
      if (newDate != null) AddApproxDateTime(dates, newDate);

      foreach (var date in dates)
      {
        if (date.DateCertainty == Certainty.Unspecified)
        {
          date.Year = reference.Year;
          date.Month = reference.Month;
          date.Day = reference.Day;
          date.DateCertainty = Certainty.Assumed;
        }
      }

      return dates.ToArray();
    }

    private static void AddApproxDateTime(List<ApproxDateTime> dates, ApproxDateTime newDate)
    {
      if (dates.Count < 1)
      {
        dates.Add(newDate);
      }
      else
      {
        var last = dates.Last();
        if (last.DateCertainty == Certainty.Unspecified && newDate.DateCertainty != Certainty.Unspecified &&
               last.TimeCertainty != Certainty.Unspecified && newDate.TimeCertainty == Certainty.Unspecified)
        {
          newDate.Hour = last.Hour;
          newDate.Minute = last.Minute;
          newDate.Second = last.Second;
          newDate.Millisecond = last.Millisecond;
          dates[dates.Count - 1] = newDate;
        }
        else if (last.DateCertainty != Certainty.Unspecified && newDate.DateCertainty == Certainty.Unspecified &&
                 last.TimeCertainty == Certainty.Unspecified && newDate.TimeCertainty != Certainty.Unspecified)
        {
          last.Hour = newDate.Hour;
          last.Minute = newDate.Minute;
          last.Second = newDate.Second;
          last.Millisecond = newDate.Millisecond;
        }
        else
        {
          dates.Add(newDate);
        }
      }
    }
    private static ApproxDateTime HandleStack(string state, List<Token> stack, DateTime reference, CultureInfo culture)
    {
      ApproxDateTime result = null;
      if (string.IsNullOrEmpty(state)) return null;
      if (!char.IsLetter(state[state.Length - 1])) state = state.Substring(0, state.Length - 1);
      switch (state)
      {
        case "D":
          if (stack[0].Text.Length == 8)
          {
            result = new ApproxDateTime();
            result.Year = int.Parse(stack[0].Text.Substring(0, 4));
            result.Month = int.Parse(stack[0].Text.Substring(4, 2));
            result.Day = int.Parse(stack[0].Text.Substring(6, 2));
          }
          else if (stack[0].Value > culture.Calendar.TwoDigitYearMax - 100 && stack[0].Value <= culture.Calendar.TwoDigitYearMax)
          {
            result = new ApproxDateTime();
            result.Year = stack[0].Value;
          }
          break;
        case "D-D":
        case "D/D":
          if (stack[0].Value > 31 && stack[2].Value >= 1 && stack[2].Value <= 12)
          {
            result = new ApproxDateTime();
            SetYear(result, culture, stack[0].Value);
            result.Month = stack[2].Value;
          }
          else if (stack[0].Value >= 1 && stack[0].Value <= 12 && stack[2].Value > 31)
          {
            result = new ApproxDateTime();
            SetYear(result, culture, stack[2].Value);
            result.Month = stack[0].Value;
          }
          else if (stack[0].Value >= 1 && stack[0].Value <= 12 && stack[2].Value >= 1 && stack[2].Value <= 31)
          {
            result = new ApproxDateTime();
            result.Year = reference.Year;
            result.CenturyCertainty = Certainty.Assumed;
            result.DecadeCertainty = Certainty.Assumed;
            result.YearNumCertainty = Certainty.Assumed;
            result.Month = stack[0].Value;
            result.Day = stack[2].Value;
          }
          break;
        case "D-D-D":
        case "D/D/D":
        case "D.D.D":
          result = new ApproxDateTime();
          if (stack[0].Value > 100)
          {
            result.Year = stack[0].Value;
            result.Month = stack[2].Value;
            result.Day = stack[4].Value;
          }
          else
          {
            result.Month = stack[0].Value;
            result.Day = stack[2].Value;
            SetYear(result, culture, stack[4].Value);
          }
          break;
        case "DDD":
          result = new ApproxDateTime();
          if (stack[0].Value > 100)
          {
            result.Year = stack[0].Value;
            result.Month = stack[1].Value;
            result.Day = stack[2].Value;
          }
          else
          {
            result.Month = stack[0].Value;
            result.Day = stack[1].Value;
            SetYear(result, culture, stack[2].Value);
          }
          break;
        case "DP":
          result = new ApproxDateTime();
          result.Hour = stack[0].Value + (stack[0].Value < 12 ? stack[1].Value * 12 : 0);
          break;
        case "D:DP":
          result = new ApproxDateTime();
          result.Hour = stack[0].Value + (stack[0].Value < 12 ? stack[3].Value * 12 : 0);
          result.Minute = stack[2].Value;
          break;
        case "D:D:DP":
          result = new ApproxDateTime();
          result.Hour = stack[0].Value + (stack[0].Value < 12 ? stack[5].Value * 12 : 0);
          result.Minute = stack[2].Value;
          result.Second = stack[4].Value;
          break;
        case "D:D:D.DP":
          result = new ApproxDateTime();
          result.Hour = stack[0].Value + (stack[0].Value < 12 ? stack[7].Value * 12 : 0);
          result.Minute = stack[2].Value;
          result.Second = stack[4].Value;
          result.Millisecond = stack[6].Value;
          break;
        case "D:D":
          result = new ApproxDateTime();
          result.Hour = stack[0].Value;
          result.Minute = stack[2].Value;
          break;
        case "D:D:D":
          result = new ApproxDateTime();
          result.Hour = stack[0].Value;
          result.Minute = stack[2].Value;
          result.Second = stack[4].Value;
          break;
        case "D:D:D.D":
          result = new ApproxDateTime();
          result.Hour = stack[0].Value;
          result.Minute = stack[2].Value;
          result.Second = stack[4].Value;
          result.Millisecond = stack[6].Value;
          break;
        case "MD":
          result = new ApproxDateTime();
          result.Month = stack[0].Value;
          if (stack[1].Value > 31)
          {
            SetYear(result, culture, stack[1].Value);
          }
          else
          {
            result.Day = stack[1].Value;
            result.Year = reference.Year;
            result.CenturyCertainty = Certainty.Assumed;
            result.DecadeCertainty = Certainty.Assumed;
            result.YearNumCertainty = Certainty.Assumed;
          }
          break;
        case "OM":
        case "DM":
          result = new ApproxDateTime();
          result.Month = stack[1].Value;
          if (stack[0].Value > 31)
          {
            SetYear(result, culture, stack[0].Value);
          }
          else
          {
            result.Day = stack[0].Value;
            result.Year = reference.Year;
            result.CenturyCertainty = Certainty.Assumed;
            result.DecadeCertainty = Certainty.Assumed;
            result.YearNumCertainty = Certainty.Assumed;
          }
          break;
        case "M'D":
          result = new ApproxDateTime();
          result.Month = stack[0].Value;
          result.Day = 1;
          SetYear(result, culture, stack[1].Value);
          break;
        case "MDD":
          result = new ApproxDateTime();
          result.Month = stack[0].Value;
          result.Day = stack[1].Value;
          SetYear(result, culture, stack[2].Value);
          break;
        case "MD,D":
        case "MO,D":
          result = new ApproxDateTime();
          result.Month = stack[0].Value;
          result.Day = stack[1].Value;
          SetYear(result, culture, stack[3].Value);
          break;
        case "MD,'D":
          result = new ApproxDateTime();
          result.Month = stack[0].Value;
          result.Day = stack[1].Value;
          SetYear(result, culture, stack[4].Value);
          break;
        case "DMD":
        case "OMD":
          result = new ApproxDateTime();
          result.Day = stack[0].Value;
          result.Month = stack[1].Value;
          SetYear(result, culture, stack[2].Value);
          break;
        case "DM'D":
        case "OM'D":
          result = new ApproxDateTime();
          result.Day = stack[0].Value;
          result.Month = stack[1].Value;
          SetYear(result, culture, stack[3].Value);
          break;
        case "year'D":
          SetYear(result, culture, stack[2].Value);
          break;
      }
      return result;
    }

    private static void SetYear(ApproxDateTime result, CultureInfo culture, int year)
    {
      if (year < 100)
      {
        int yearCalc = culture.Calendar.TwoDigitYearMax / 100 * 100 + year;
        if (yearCalc > culture.Calendar.TwoDigitYearMax) yearCalc -= 100;
        result.Year = yearCalc;
        result.CenturyCertainty = Certainty.Assumed;
      }
      else
      {
        result.Year = year;
      }
    }

    private static IEnumerable<Token> GetInterpretedTokens(string value)
    {
      Token lastToken = null;
      Token newToken;
      foreach (var token in Tokenize(value))
      {
        newToken = InterpretToken(token);
        if (newToken.Type == TokenType.OrdinalSuffix)
        {
          if (lastToken != null && lastToken.Type == TokenType.Digit &&
              (newToken.Value == 0 || lastToken.Value % 10 == newToken.Value))
          {
            lastToken.Type = TokenType.Ordinal;
            newToken = null;
          }
          else
          {
            newToken.Type = TokenType.Text;
          }
        }

        if (lastToken != null)
        {
          yield return lastToken;
        }
        lastToken = newToken;
      }
      if (lastToken != null)
      {
        yield return lastToken;
      }
    }
    private static Token InterpretToken(Token token)
    {
      var result = token;
      switch (token.Type)
      {
        case TokenType.Digit:
          result.Value = int.Parse(result.Text);
          break;
        case TokenType.Text:
          if (!_lookups.TryGetValue(token.Text, out result))
          {
            result = token;
            if (token.Text.Length >= 5)
            {
              var cmp = (from c in token.Text select (int)c).ToArray();
              foreach (var check in _spellingChecks)
              {
                if (DamerauLevenshteinDistance(cmp, check.Key, 5) <= (token.Text.Length / 5))
                {
                  result = check.Value;
                  break;
                }
              }
            }
          }
          break;
      }

      return result;
    }

    private static Dictionary<string, Token> _lookups;
    private static KeyValuePair<int[], Token>[] _spellingChecks;

    private static void AddLookup(TokenType type, string text, int value)
    {
      _lookups[text.ToLowerInvariant()] = new Token(type) { Text = text.ToLowerInvariant(), Value = value};
    }
    private static void AddLookups(TokenType type, string[] strings, Func<int, int> valueGetter, int count)
    {
      string str;
      count = Math.Min(count, strings.Length);
      for (int i = 0; i < count; i++)
      {
        str = strings[i].ToLowerInvariant();
        _lookups[str] = new Token(type) { Text = str, Value = valueGetter.Invoke(i) };
      }
    }

    private static void EnsureLookups(CultureInfo culture)
    {
      if (_lookups == null)
      {
        _lookups = new Dictionary<string, Token>();

        AddLookups(TokenType.Weekday, culture.DateTimeFormat.AbbreviatedDayNames, (x) => x, 7);
        AddLookups(TokenType.Weekday, culture.DateTimeFormat.DayNames, (x) => x, 7);
        AddLookups(TokenType.Month, culture.DateTimeFormat.AbbreviatedMonthGenitiveNames, (x) => x + 1, 12);
        AddLookups(TokenType.Month, culture.DateTimeFormat.AbbreviatedMonthNames, (x) => x + 1, 12);
        AddLookups(TokenType.Month, culture.DateTimeFormat.MonthGenitiveNames, (x) => x + 1, 12);
        AddLookups(TokenType.Month, culture.DateTimeFormat.MonthNames, (x) => x + 1, 12);
        AddLookup(TokenType.Meridian, culture.DateTimeFormat.AMDesignator, 0);
        AddLookup(TokenType.Meridian, culture.DateTimeFormat.PMDesignator, 1);

        if (culture.IetfLanguageTag == "en-US")
        {
          AddLookup(TokenType.Meridian, "a.m.", 0);
          AddLookup(TokenType.Meridian, "p.m.", 1);
          AddLookup(TokenType.OrdinalSuffix, "th", 0);
          AddLookup(TokenType.OrdinalSuffix, "st", 1);
          AddLookup(TokenType.OrdinalSuffix, "nd", 2);
          AddLookup(TokenType.OrdinalSuffix, "rd", 3);
          AddLookup(TokenType.Number, "zero", 0);
          AddLookup(TokenType.Number, "one", 1);
          AddLookup(TokenType.Number, "two", 2);
          AddLookup(TokenType.Number, "three", 3);
          AddLookup(TokenType.Number, "four", 4);
          AddLookup(TokenType.Number, "five", 5);
          AddLookup(TokenType.Number, "six", 6);
          AddLookup(TokenType.Number, "seven", 7);
          AddLookup(TokenType.Number, "eight", 8);
          AddLookup(TokenType.Number, "nine", 9);
          AddLookup(TokenType.Number, "ten", 10);
          AddLookup(TokenType.Number, "eleven", 11);
          AddLookup(TokenType.Number, "twelve", 12);
          AddLookup(TokenType.Number, "thirteen", 13);
          AddLookup(TokenType.Number, "fourteen", 14);
          AddLookup(TokenType.Number, "fifteen", 15);
          AddLookup(TokenType.Number, "sixteen", 16);
          AddLookup(TokenType.Number, "seventeen", 17);
          AddLookup(TokenType.Number, "eighteen", 18);
          AddLookup(TokenType.Number, "nineteen", 19);
          AddLookup(TokenType.Number, "twenty", 20);
          AddLookup(TokenType.Number, "twenty-one", 21);
          AddLookup(TokenType.Number, "twenty-two", 22);
          AddLookup(TokenType.Number, "twenty-three", 23);
          AddLookup(TokenType.Number, "twenty-four", 24);
          AddLookup(TokenType.Number, "twenty-five", 25);
          AddLookup(TokenType.Number, "twenty-six", 26);
          AddLookup(TokenType.Number, "twenty-seven", 27);
          AddLookup(TokenType.Number, "twenty-eight", 28);
          AddLookup(TokenType.Number, "twenty-nine", 29);
          AddLookup(TokenType.Number, "thirty", 30);
          AddLookup(TokenType.Number, "thirty-one", 31);
          AddLookup(TokenType.Number, "thirty-two", 32);
          AddLookup(TokenType.Number, "thirty-three", 33);
          AddLookup(TokenType.Number, "thirty-four", 34);
          AddLookup(TokenType.Number, "thirty-five", 35);
          AddLookup(TokenType.Number, "thirty-six", 36);
          AddLookup(TokenType.Number, "thirty-seven", 37);
          AddLookup(TokenType.Number, "thirty-eight", 38);
          AddLookup(TokenType.Number, "thirty-nine", 39);
          AddLookup(TokenType.Number, "forty", 40);
          AddLookup(TokenType.Number, "forty-one", 41);
          AddLookup(TokenType.Number, "forty-two", 42);
          AddLookup(TokenType.Number, "forty-three", 43);
          AddLookup(TokenType.Number, "forty-four", 44);
          AddLookup(TokenType.Number, "forty-five", 45);
          AddLookup(TokenType.Number, "forty-six", 46);
          AddLookup(TokenType.Number, "forty-seven", 47);
          AddLookup(TokenType.Number, "forty-eight", 48);
          AddLookup(TokenType.Number, "forty-nine", 49);
          AddLookup(TokenType.Number, "fifty", 50);
          AddLookup(TokenType.Number, "fifty-one", 51);
          AddLookup(TokenType.Number, "fifty-two", 52);
          AddLookup(TokenType.Number, "fifty-three", 53);
          AddLookup(TokenType.Number, "fifty-four", 54);
          AddLookup(TokenType.Number, "fifty-five", 55);
          AddLookup(TokenType.Number, "fifty-six", 56);
          AddLookup(TokenType.Number, "fifty-seven", 57);
          AddLookup(TokenType.Number, "fifty-eight", 58);
          AddLookup(TokenType.Number, "fifty-nine", 59);
          AddLookup(TokenType.Number, "sixty", 60);
          AddLookup(TokenType.Number, "sixty-one", 61);
          AddLookup(TokenType.Number, "sixty-two", 62);
          AddLookup(TokenType.Number, "sixty-three", 63);
          AddLookup(TokenType.Number, "sixty-four", 64);
          AddLookup(TokenType.Number, "sixty-five", 65);
          AddLookup(TokenType.Number, "sixty-six", 66);
          AddLookup(TokenType.Number, "sixty-seven", 67);
          AddLookup(TokenType.Number, "sixty-eight", 68);
          AddLookup(TokenType.Number, "sixty-nine", 69);
          AddLookup(TokenType.Number, "seventy", 70);
          AddLookup(TokenType.Number, "seventy-one", 71);
          AddLookup(TokenType.Number, "seventy-two", 72);
          AddLookup(TokenType.Number, "seventy-three", 73);
          AddLookup(TokenType.Number, "seventy-four", 74);
          AddLookup(TokenType.Number, "seventy-five", 75);
          AddLookup(TokenType.Number, "seventy-six", 76);
          AddLookup(TokenType.Number, "seventy-seven", 77);
          AddLookup(TokenType.Number, "seventy-eight", 78);
          AddLookup(TokenType.Number, "seventy-nine", 79);
          AddLookup(TokenType.Number, "eighty", 80);
          AddLookup(TokenType.Number, "eighty-one", 81);
          AddLookup(TokenType.Number, "eighty-two", 82);
          AddLookup(TokenType.Number, "eighty-three", 83);
          AddLookup(TokenType.Number, "eighty-four", 84);
          AddLookup(TokenType.Number, "eighty-five", 85);
          AddLookup(TokenType.Number, "eighty-six", 86);
          AddLookup(TokenType.Number, "eighty-seven", 87);
          AddLookup(TokenType.Number, "eighty-eight", 88);
          AddLookup(TokenType.Number, "eighty-nine", 89);
          AddLookup(TokenType.Number, "ninety", 90);
          AddLookup(TokenType.Number, "ninety-one", 91);
          AddLookup(TokenType.Number, "ninety-two", 92);
          AddLookup(TokenType.Number, "ninety-three", 93);
          AddLookup(TokenType.Number, "ninety-four", 94);
          AddLookup(TokenType.Number, "ninety-five", 95);
          AddLookup(TokenType.Number, "ninety-six", 96);
          AddLookup(TokenType.Number, "ninety-seven", 97);
          AddLookup(TokenType.Number, "ninety-eight", 98);
          AddLookup(TokenType.Number, "ninety-nine", 99);
          AddLookup(TokenType.Ordinal, "first", 1);
          AddLookup(TokenType.Ordinal, "second", 2);
          AddLookup(TokenType.Ordinal, "third", 3);
          AddLookup(TokenType.Ordinal, "fourth", 4);
          AddLookup(TokenType.Ordinal, "fifth", 5);
          AddLookup(TokenType.Ordinal, "sixth", 6);
          AddLookup(TokenType.Ordinal, "seventh", 7);
          AddLookup(TokenType.Ordinal, "eighth", 8);
          AddLookup(TokenType.Ordinal, "ninth", 9);
          AddLookup(TokenType.Ordinal, "tenth", 10);
          AddLookup(TokenType.Ordinal, "eleventh", 11);
          AddLookup(TokenType.Ordinal, "twelfth", 12);
          AddLookup(TokenType.Ordinal, "thirteenth", 13);
          AddLookup(TokenType.Ordinal, "fourteenth", 14);
          AddLookup(TokenType.Ordinal, "fifteenth", 15);
          AddLookup(TokenType.Ordinal, "sixteenth", 16);
          AddLookup(TokenType.Ordinal, "seventeenth", 17);
          AddLookup(TokenType.Ordinal, "eighteenth", 18);
          AddLookup(TokenType.Ordinal, "nineteenth", 19);
          AddLookup(TokenType.Ordinal, "twentieth", 20);
          AddLookup(TokenType.Ordinal, "twenty-first", 21);
          AddLookup(TokenType.Ordinal, "twenty-second", 22);
          AddLookup(TokenType.Ordinal, "twenty-third", 23);
          AddLookup(TokenType.Ordinal, "twenty-fourth", 24);
          AddLookup(TokenType.Ordinal, "twenty-fifth", 25);
          AddLookup(TokenType.Ordinal, "twenty-sixth", 26);
          AddLookup(TokenType.Ordinal, "twenty-seventh", 27);
          AddLookup(TokenType.Ordinal, "twenty-eighth", 28);
          AddLookup(TokenType.Ordinal, "twenty-ninth", 29);
          AddLookup(TokenType.Ordinal, "thirtieth", 30);
          AddLookup(TokenType.Ordinal, "thirty-first", 31);

          //_lookups["immediately"] = 'N';
          //_lookups["today"] = 'N';
          //_lookups["now"] = 'N';
        }

        _spellingChecks = (from k in _lookups
                           where k.Key.Length > 3
                           select new KeyValuePair<int[], Token>((from c in k.Key select (int)c).ToArray(), k.Value)).ToArray();
      }
    }

    public static IEnumerable<Token> Tokenize(string value)
    {
      if (string.IsNullOrEmpty(value)) yield break;

      int start = 0;
      while (start < value.Length && char.IsWhiteSpace(value[start])) start++;
      if (start >= value.Length) yield break;

      var currToken = new Token();
      if (char.IsLetter(value[start]))
      {
        currToken.Type = TokenType.Text;
      }
      else if (char.IsDigit(value[start]))
      {
        currToken.Type = TokenType.Digit;
      }
      else
      {
        currToken.Type = TokenType.Punctuation;
      }

      for (int i = start + 1; i < value.Length; i++)
      {
        if (char.IsLetter(value[i]))
        {
          if (currToken.Type == TokenType.Unknown)
          {
            currToken.Type = TokenType.Text;
          }
          else if (currToken.Type != TokenType.Text)
          {
            currToken.Text = value.Substring(start, i - start);
            yield return currToken;
            start = i;
            currToken = new Token(TokenType.Text);
          }
        }
        else if (char.IsNumber(value[i]))
        {
          if (currToken.Type == TokenType.Unknown)
          {
            currToken.Type = TokenType.Digit;
          }
          else if (currToken.Type != TokenType.Digit)
          {
            currToken.Text = value.Substring(start, i - start);
            yield return currToken;
            start = i;
            currToken = new Token(TokenType.Digit);
          }
        }
        else if (char.IsWhiteSpace(value[i]))
        {
          if (start < i)
          {
            currToken.Text = value.Substring(start, i - start);
            yield return currToken;
            currToken = new Token(TokenType.Unknown);
          }
          start = i + 1;
        }
        else
        {
          if (currToken.Type == TokenType.Text && (value[i] == '.' || 
                                                  char.GetUnicodeCategory(value[i]) == System.Globalization.UnicodeCategory.DashPunctuation || 
                                                  char.GetUnicodeCategory(value[i]) == System.Globalization.UnicodeCategory.ConnectorPunctuation))
          {
            // Do Nohting
          }
          else if (currToken.Type == TokenType.Unknown)
          {
            currToken.Type = TokenType.Punctuation;
          }
          else {
            currToken.Text = value.Substring(start, i - start);
            yield return currToken;
            start = i;
            currToken = new Token(TokenType.Punctuation);
          }
        }
      }

      if (currToken.Type != TokenType.Unknown)
      {
        currToken.Text = value.Substring(start, value.Length - start);
        yield return currToken;
      }
    }

    /// <summary>
    /// Computes the Damerau-Levenshtein Distance between two strings, represented as arrays of
    /// integers, where each integer represents the code point of a character in the source string.
    /// Includes an optional threshhold which can be used to indicate the maximum allowable distance.
    /// </summary>
    /// <param name="source">An array of the code points of the first string</param>
    /// <param name="target">An array of the code points of the second string</param>
    /// <param name="threshold">Maximum allowable distance</param>
    /// <returns>Int.MaxValue if threshhold exceeded; otherwise the Damerau-Leveshteim distance between the strings</returns>
    private static int DamerauLevenshteinDistance(int[] source, int[] target, int threshold)
    {

      int length1 = source.Length;
      int length2 = target.Length;

      // Return trivial case - difference in string lengths exceeds threshhold
      if (Math.Abs(length1 - length2) > threshold) { return int.MaxValue; }

      // Ensure arrays [i] / length1 use shorter length 
      if (length1 > length2)
      {
        Swap(ref target, ref source);
        Swap(ref length1, ref length2);
      }

      int maxi = length1;
      int maxj = length2;

      int[] dCurrent = new int[maxi + 1];
      int[] dMinus1 = new int[maxi + 1];
      int[] dMinus2 = new int[maxi + 1];
      int[] dSwap;

      for (int i = 0; i <= maxi; i++) { dCurrent[i] = i; }

      int jm1 = 0, im1 = 0, im2 = -1;

      for (int j = 1; j <= maxj; j++)
      {

        // Rotate
        dSwap = dMinus2;
        dMinus2 = dMinus1;
        dMinus1 = dCurrent;
        dCurrent = dSwap;

        // Initialize
        int minDistance = int.MaxValue;
        dCurrent[0] = j;
        im1 = 0;
        im2 = -1;

        for (int i = 1; i <= maxi; i++)
        {

          int cost = source[im1] == target[jm1] ? 0 : 1;

          int del = dCurrent[im1] + 1;
          int ins = dMinus1[i] + 1;
          int sub = dMinus1[im1] + cost;

          //Fastest execution for min value of 3 integers
          int min = (del > ins) ? (ins > sub ? sub : ins) : (del > sub ? sub : del);

          if (i > 1 && j > 1 && source[im2] == target[jm1] && source[im1] == target[j - 2])
            min = Math.Min(min, dMinus2[im2] + cost);

          dCurrent[i] = min;
          if (min < minDistance) { minDistance = min; }
          im1++;
          im2++;
        }
        jm1++;
        if (minDistance > threshold) { return int.MaxValue; }
      }

      int result = dCurrent[maxi];
      return (result > threshold) ? int.MaxValue : result;
    }

    private static void Swap<T>(ref T arg1, ref T arg2)
    {
      T temp = arg1;
      arg1 = arg2;
      arg2 = temp;
    }
  }
}
