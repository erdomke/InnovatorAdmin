using Pipes.Css.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Css
{
  public class GlobalStyleContext
  {
    public double Dpi { get; set; }
    public bool LeftToRight { get; set; }
    public MediaType Media { get; set; }
    public int MediaWidth { get; set; }
    public Func<string, string> ResourceLoader { get; set; }
    public double ViewportWidth { get; set; }
    public double ViewportHeight { get; set; }


    public GlobalStyleContext()
    {
      this.Dpi = 96.0;
      this.LeftToRight = true;
      this.Media = MediaType.Screen;
      this.MediaWidth = 1200;
    }
  }
}
