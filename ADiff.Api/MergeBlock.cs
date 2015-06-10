using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADiff
{
  public class MergeBlock
  {
    private List<MergeAlternate> _alternates = new List<MergeAlternate>();

    public IList<MergeAlternate> Alternates
    {
      get { return _alternates; }
    }
    public bool IsConflict { get; set; }

    public override string ToString()
    {
      if (_alternates.Any(a => (a.Location & MergeLocation.Output) > 0))
      {
        return _alternates.First(a => (a.Location & MergeLocation.Output) > 0).ToString();
      }
      else
      {
        return "<<<<<<< LEFT" + Environment.NewLine
          + _alternates.First(a => (a.Location & MergeLocation.Left) > 0).ToString()
          + "=======" + Environment.NewLine
          + _alternates.First(a => (a.Location & MergeLocation.Right) > 0).ToString()
          + ">>>>>>> RIGHT" + Environment.NewLine;
      }
    }
  }
}
