using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADiff.Api
{
  public class MergeAlternate
  {
    public string Text { get; set; }
    public MergeLocation Location { get; set; }
    public int LineCount { get; set; }

    public override string ToString()
    {
      return this.Text;
    }
  }
}
