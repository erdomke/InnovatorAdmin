using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml
{
  public interface ISgmlTextWriterSettings
  {
    System.Xml.ConformanceLevel ConformanceLevel { get; set; }
    bool Indent { get; set; }
    string IndentChars { get; set; }
    string NewLineChars { get; set; }
    bool NewLineOnAttributes { get; set; }
  }
}
