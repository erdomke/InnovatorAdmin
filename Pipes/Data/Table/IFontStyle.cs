using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface IFontStyle
  {
    bool Bold { get; set; }
    bool Italic { get; set; }
    string Name { get; set; }
    float SizeInPoints { get; set; }
    bool Strikeout { get; set; }
    bool Underline { get; set; }
  }
}
