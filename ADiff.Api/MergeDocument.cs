using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADiff.Api
{
  public class MergeDocument
  {
    private List<MergeBlock> _blocks = new List<MergeBlock>();

    public IList<MergeBlock> Blocks
    {
      get { return _blocks; }
    }

    public override string ToString()
    {
      var builder = new StringBuilder();
      foreach (var block in _blocks)
      {
        builder.Append(block.ToString());
      }
      return builder.ToString();
    }
  }
}
