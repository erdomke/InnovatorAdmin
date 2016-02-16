using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data.Table
{
  public interface IHyperlink
  {
    Uri Target { get; }
    string Text { get; }
  }
}
