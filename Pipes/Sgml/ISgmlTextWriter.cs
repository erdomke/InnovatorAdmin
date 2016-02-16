using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Pipes.Sgml
{
  public interface ISgmlTextWriter : ISgmlWriter, IWriter<TextWriter>
  {
    TextWriter Raw();
    ISgmlWriter RawEnd();
    TextWriter Value();
    ISgmlWriter ValueEnd();
  }
}
