using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace InnovatorAdmin.Editor
{
  public class SearchResult : TextSegment, ISearchResult, ISegment
  {
    public Match Data { get; set; }

    public string ReplaceWith(string replacement)
    {
      return this.Data.Result(replacement);
    }
  }
}
