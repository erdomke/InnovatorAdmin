using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes
{
  public interface IPipeOutput<T> : IEnumerable<T>
  {
  }
}
