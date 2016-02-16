using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Code
{
  public interface IImperativeCodeWriter : IBaseCodeWriter
  {
    IImperativeCodeWriter LineEnd();
    IImperativeCodeWriter LineContinue();
  }
}
