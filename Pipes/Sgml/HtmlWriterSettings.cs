using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml
{
  public class HtmlWriterSettings : ISgmlTextWriterSettings
  {
    public System.Xml.ConformanceLevel ConformanceLevel { get; set; }
    public bool Indent { get; set; }
    public string IndentChars { get; set; }
    public string NewLineChars { get; set; }
    public bool NewLineOnAttributes { get; set; }
    public bool ReplaceConsecutiveSpaceNonBreaking { get; set; }

    public HtmlWriterSettings()
    {
      this.Indent = false;
      this.IndentChars = "  ";
      this.NewLineChars = Environment.NewLine;
      this.NewLineOnAttributes = false;
      this.ReplaceConsecutiveSpaceNonBreaking = false;
    }
  }
}
