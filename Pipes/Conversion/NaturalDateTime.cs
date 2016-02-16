using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Pipes.Conversion
{
  public class NaturalDateTime
  {
    private enum CharType
    {
      Letter, Digit, Other
    }

    public static DateTime Parse(string input)
    {
      return Parse(input, DateTime.Now, CultureInfo.CurrentCulture);
    }
    public static bool TryParse(string input, out DateTime result)
    {
      return TryParse(input, DateTime.Now, CultureInfo.CurrentCulture, out result);
    }
    public static DateTime Parse(string input, DateTime relativeBase)
    {
      return Parse(input, relativeBase, CultureInfo.CurrentCulture);
    }
    public static bool TryParse(string input, DateTime relativeBase, out DateTime result)
    {
      return TryParse(input, relativeBase, CultureInfo.CurrentCulture, out result);
    }

    public static DateTime Parse(string input, DateTime relativeBase, CultureInfo culture)
    {
      DateTime result;
      if (!TryParse(input, relativeBase, culture, out result))
      {
        throw new FormatException();
      }
      return result;
    }
    public static bool TryParse(string input, DateTime relativeBase, CultureInfo culture, out DateTime result)
    {
      result = DateTime.MinValue;
      if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(input.Trim())) return false;
      EnsureLookups(culture);

      input = input.Trim().ToLowerInvariant();
      var parts = new List<string>();
      var builder = new StringBuilder();
      builder.Append(input[0]);
      var lastType = GetCharType(input[0]);
      CharType currType;
      var pattern = new StringBuilder();
      string normalized = null;

      for (int i = 1; i < input.Length; i++)
      {
        currType = GetCharType(input[i]);
        if (currType != lastType || currType == CharType.Other)
        {
          pattern.Append(GetSegmentType(builder.ToString(), out normalized));
          parts.Add(normalized);
          builder.Length = 0;
        }
        builder.Append(input[i]);
        lastType = currType;
      }
      pattern.Append(GetSegmentType(builder.ToString(), out normalized));
      parts.Add(normalized);

      var finalParts = parts.ToArray();
      var datePart = DateTime.MinValue;
      var dateFound = false;
      dateFound = dateFound || TestMatch(pattern.ToString(), "D[-/]D[-/]D[-/]D",
        (m) => DateTime.TryParse(string.Join("", finalParts, m.Index, 3) + finalParts[m.Index + 1] + finalParts[m.Index + 6], out datePart));
      dateFound = dateFound || TestMatch(pattern.ToString(), "D[-/]D[-/]D", 
        (m) => DateTime.TryParse(string.Join("", finalParts, m.Index, 5), out datePart) );
      dateFound = dateFound || TestMatch(pattern.ToString(), "D[-/]D", 
        (m) => DateTime.TryParse(string.Join("", finalParts, m.Index, 3) + finalParts[m.Index + 1] + relativeBase.Year.ToString(), out datePart));
      dateFound = dateFound || TestMatch(pattern.ToString(), @"(M)\W*(D)O?(\W+(D))?",
        (m) => {
          if (int.Parse(finalParts[m.Groups[2].Index]) > 31)
          {
            return DateTime.TryParse(finalParts[m.Groups[1].Index] + " 1, " + finalParts[m.Groups[2].Index], out datePart);
          }
          else
          {
            return DateTime.TryParse(finalParts[m.Groups[1].Index] + " " + finalParts[m.Groups[2].Index] + ", " + (m.Groups[4].Success ? finalParts[m.Groups[4].Index] : relativeBase.Year.ToString()), out datePart);
          }
        });

      DateTime timePart = DateTime.MinValue;
      var timeFound = false;
      timeFound = timeFound || TestMatch(pattern.ToString(), @"D(:D)(:D)?(\W*(P))?",
        (m) => {
          if (!m.Groups[1].Success && !m.Groups[3].Success) return false;
          var start = string.Join("", finalParts, m.Index, (m.Groups[2].Success ? 5 : (m.Groups[1].Success ? 3 : 1)));
          return DateTime.TryParse(start + (m.Groups[4].Success ? finalParts[m.Groups[4].Index] : ""), out timePart);
        });
      timeFound = timeFound || TestMatch(pattern.ToString(), @"D\W*(P)",
        (m) =>
        {
          return DateTime.TryParse(finalParts[m.Index] + ":00" + finalParts[m.Groups[1].Index], out timePart);
        });


      if (timeFound)
      {
        if (dateFound)
        {
          datePart = datePart.AddMilliseconds(timePart.TimeOfDay.TotalMilliseconds);
        }
        else
        {
          datePart = relativeBase.Date.AddMilliseconds(timePart.TimeOfDay.TotalMilliseconds);
        }
      }

      result = datePart;
      return timeFound || dateFound;
    }

    private static bool TestMatch(string input, string pattern, Func<Match, bool> func)
    {
      var match = Regex.Match(input, pattern);
      if (match.Success)
      {
        return func.Invoke(match);
      }
      return false;
    }

    //private static string[] _weekDays = null;
    //private static string[] _months = null;
    private static Dictionary<string, char> _lookups;
    private static KeyValuePair<int[], char>[] _spellingChecks;

    /// <summary>
    /// D = digit
    /// W = weekday
    /// N = now
    /// M = month
    /// O = ordinal
    /// P = meridian
    /// t = Text
    /// </summary>
    /// <param name="culture"></param>
    private static char GetSegmentType(string value, out string normalized)
    {
      normalized = value;
      if (string.IsNullOrEmpty(value)) return '0';

      char result = '\0';
      if (!_lookups.TryGetValue(value, out result))
      {
        int val;
        if (int.TryParse(value, out val))
        {
          result = 'D';
        }
        else if (value.Length >= 5)
        {
          var cmp = (from c in value select (int)c).ToArray();
          foreach (var check in _spellingChecks)
          {
            if (DamerauLevenshteinDistance(cmp, check.Key, 5) <= (value.Length / 5))
            {
              normalized = new string((from i in check.Key select (char)i).ToArray());
              result = check.Value;
              break;
            }
          }
        }
      }

      if (result == '\0')
      {
        if (char.IsLetter(value[0]))
        {
          return 't';
        }
        else
        {
          return value[0];
        }
      }
      else
      {
        return result;
      }
    }
    
    private static void EnsureLookups(CultureInfo culture)
    {
      if (_lookups == null)
      {
        _lookups = new Dictionary<string,char>();
        foreach (var w in culture.DateTimeFormat.AbbreviatedDayNames.Union(culture.DateTimeFormat.DayNames).Distinct())
        {
          _lookups[w.ToLowerInvariant()] = 'W';
        }

        foreach(var w in culture.DateTimeFormat.AbbreviatedMonthGenitiveNames
                          .Union(culture.DateTimeFormat.AbbreviatedMonthNames)
                          .Union(culture.DateTimeFormat.MonthGenitiveNames)
                          .Union(culture.DateTimeFormat.MonthNames).Distinct().Where((m) => !string.IsNullOrEmpty(m)))
        {
          _lookups[w.ToLowerInvariant()] = 'M';
        }
        _lookups[culture.DateTimeFormat.AMDesignator.ToLowerInvariant()] = 'P';
        _lookups[culture.DateTimeFormat.PMDesignator.ToLowerInvariant()] = 'P';

        if (culture.IetfLanguageTag == "en-US")
        {
          _lookups["st"] = 'O';
          _lookups["nd"] = 'O';
          _lookups["rd"] = 'O';
          _lookups["immediately"] = 'N';
          _lookups["today"] = 'N';
          _lookups["now"] = 'N';
        }

        _spellingChecks = (from k in _lookups
                           where k.Key.Length > 3
                           select new KeyValuePair<int[], char>((from c in k.Key select (int)c).ToArray(), k.Value)).ToArray();
      }
    }

    private static CharType GetCharType(char value)
    {
      if (Char.IsLetter(value))
      {
        return CharType.Letter;
      }
      else if (Char.IsDigit(value))
      {
        return CharType.Digit;
      }
      else
      {
        return CharType.Other;
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
    public static int DamerauLevenshteinDistance(int[] source, int[] target, int threshold)
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

    static void Swap<T>(ref T arg1, ref T arg2)
    {
      T temp = arg1;
      arg1 = arg2;
      arg2 = temp;
    }
  }
}
