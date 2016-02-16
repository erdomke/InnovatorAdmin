using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes
{
  public interface IPipeInput<T>
  {
    void Initialize(IEnumerable<T> source);
  }
}
