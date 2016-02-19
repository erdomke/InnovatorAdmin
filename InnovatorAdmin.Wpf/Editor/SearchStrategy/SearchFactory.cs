using ICSharpCode.AvalonEdit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public static class SearchFactory
  {
    /// <summary>
    /// Creates a default ISearchStrategy with the given parameters.
    /// </summary>
    public static ISearchStrategy Create(string searchPattern, bool ignoreCase, bool matchWholeWords, SearchMode mode)
    {
      if (searchPattern == null)
      {
        throw new ArgumentNullException("searchPattern");
      }

      var regexOptions = RegexOptions.Compiled;
      if ((mode == SearchMode.Extended || mode == SearchMode.RegEx)
        && (searchPattern.IndexOf("\\r") > 0 || searchPattern.IndexOf("\\n") > 0))
      {
        // Don't change the Regex mode
      }
      else
      {
        regexOptions |= RegexOptions.Multiline;
      }

      if (ignoreCase)
      {
        regexOptions |= RegexOptions.IgnoreCase;
      }

      switch (mode)
      {
        case SearchMode.Normal:
          searchPattern = Regex.Escape(searchPattern);
          break;
        case SearchMode.Extended:
          try
          {
            searchPattern = Regex.Escape(StringHelper.StringFromCSharpLiteral(searchPattern));
          }
          catch (ArgumentException ex)
          {
            throw new SearchPatternException(ex.Message, ex);
          }
          break;
        case SearchMode.Wildcard:
          searchPattern = ConvertWildcardsToRegex(searchPattern);
          break;
        case SearchMode.XPath:
          return new XPathSearchStrategy(searchPattern);
      }

      try
      {
        var searchPattern2 = new Regex(searchPattern, regexOptions);
        return new RegexSearchStrategy(searchPattern2, matchWholeWords);
      }
      catch (ArgumentException ex)
      {
        throw new SearchPatternException(ex.Message, ex);
      }
    }

    private static string ConvertWildcardsToRegex(string searchPattern)
    {
      if (string.IsNullOrEmpty(searchPattern))
      {
        return "";
      }

      var stringBuilder = new StringBuilder();
      for (int i = 0; i < searchPattern.Length; i++)
      {
        char c = searchPattern[i];
        char c2 = c;
        if (c2 != '*')
        {
          if (c2 == '?')
          {
            stringBuilder.Append(".");
          }
          else
          {
            stringBuilder.Append(Regex.Escape(c.ToString()));
          }
        }
        else
        {
          stringBuilder.Append(".*");
        }
      }
      return stringBuilder.ToString();
    }
  }
}
