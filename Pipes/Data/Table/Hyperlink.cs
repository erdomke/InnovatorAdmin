using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public class Hyperlink : IHyperlink
  {
    public Uri Target { get; set; }
    public string Text { get; set; }
  }
}
