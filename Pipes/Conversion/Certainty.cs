using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Conversion
{
  [Flags()]
  public enum Certainty
  {
    Certain = 0,
    Assumed = 1,
    Approximate = 2,
    Uncertain = 4,
    Unspecified = 8,
    Unknowable = 16
  }
}
