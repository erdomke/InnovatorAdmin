using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADiff
{
  public class MergeAlternate
  {
    private MergeBlock _parent;

    public MergeBlock Parent { get { return _parent; } }
    public string Text { get; set; }
    public MergeLocation Location { get; set; }
    public int LineCount { get; set; }

    public MergeAlternate(MergeBlock parent)
    {
      _parent = parent;
    }

    public override string ToString()
    {
      return this.Text;
    }
  }
}
