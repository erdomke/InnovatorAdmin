using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Sgml
{
  public interface ISgmlGroupWriter : ISgmlWriter
  {
    ISgmlGroupWriter ElementGroup(string name);
    ISgmlGroupWriter ElementGroupEnd();
  }
}
