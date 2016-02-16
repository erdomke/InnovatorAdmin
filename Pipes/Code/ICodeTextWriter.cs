using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pipes.Code
{
  public interface ICodeTextWriter : Code.IBaseCodeWriter
  {
    TextWriter Raw();
    ICodeTextWriter RawEnd();
    TextWriter StringValue();
    ICodeTextWriter StringValueEnd();
  }
}
