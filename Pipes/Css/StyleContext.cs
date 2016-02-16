using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Css
{
  public class StyleContext : ICloneable
  {
    public double FontSizePx { get; set; }
    public double ContentWidth { get; set; }
    public GlobalStyleContext Global { get; set; }

    public StyleContext()
    {
      this.FontSizePx = 16;
      this.ContentWidth = 1200;
    }

    public StyleContext Clone()
    {
      return new StyleContext()
      {
        ContentWidth = this.ContentWidth,
        FontSizePx = this.FontSizePx
      };
    }
    object ICloneable.Clone()
    {
      return this.Clone();
    }
  }
}
