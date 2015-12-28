using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace InnovatorAdmin.Editor
{
  public class RegexSearchStrategy : ISearchStrategy, IEquatable<ISearchStrategy>
  {
    private readonly Regex searchPattern;
    private readonly bool matchWholeWords;

    public RegexSearchStrategy(Regex searchPattern, bool matchWholeWords)
    {
      if (searchPattern == null)
      {
        throw new ArgumentNullException("searchPattern");
      }
      this.searchPattern = searchPattern;
      this.matchWholeWords = matchWholeWords;
    }

    public IEnumerable<ISearchResult> FindAll(ITextSource document, int offset, int length)
    {
      var num = offset + length;
      foreach (Match match in this.searchPattern.Matches(document.Text))
      {
        var num2 = match.Length + match.Index;
        if (offset <= match.Index
          && num >= num2
          && (!this.matchWholeWords
            || (RegexSearchStrategy.IsWordBorder(document, match.Index)
              && RegexSearchStrategy.IsWordBorder(document, num2))))
        {
          yield return new SearchResult
          {
            StartOffset = match.Index,
            Length = match.Length,
            Data = match
          };
        }
      }
      yield break;
    }
    private static bool IsWordBorder(ITextSource document, int offset)
    {
      return TextUtilities.GetNextCaretPosition(document, offset - 1, LogicalDirection.Forward, CaretPositioningMode.WordBorder) == offset;
    }
    public ISearchResult FindNext(ITextSource document, int offset, int length)
    {
      return this.FindAll(document, offset, length).FirstOrDefault<ISearchResult>();
    }
    public bool Equals(ISearchStrategy other)
    {
      var regexSearchStrategy = other as RegexSearchStrategy;
      return regexSearchStrategy != null
        && regexSearchStrategy.searchPattern.ToString() == this.searchPattern.ToString()
        && regexSearchStrategy.searchPattern.Options == this.searchPattern.Options
        && regexSearchStrategy.searchPattern.RightToLeft == this.searchPattern.RightToLeft;
    }

    private class SearchResult : TextSegment, ISearchResult, ISegment
    {
      public Match Data { get; set; }

      public string ReplaceWith(string replacement)
      {
        return this.Data.Result(replacement);
      }
    }
  }
}
