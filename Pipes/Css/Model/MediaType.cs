using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Css.Model
{
  [Flags()]
  public enum MediaType
  {
    Screen = 1,
    Print = 2,
    Speech = 4,
    Braille = 8,
    Embossed = 16,
    Handheld = 32,
    Projection = 64,
    Tty = 128,
    Tv = 256,
    All = -1
  }
}
