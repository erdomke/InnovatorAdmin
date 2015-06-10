using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADiff
{
  public struct VerticalOffset
  {
    public int BlockIndex;
    public double PercentOffset;

    public VerticalOffset(int blockIndex, double percentOffset)
    {
      this.BlockIndex = blockIndex;
      this.PercentOffset = percentOffset;
    }
  }
}
