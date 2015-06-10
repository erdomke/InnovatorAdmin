using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADiff
{
  [Flags]
  public enum MergeLocation
  {
    Right = 1,
    Parent = 2,
    Left = 4,
    Output = 8
  }
}
