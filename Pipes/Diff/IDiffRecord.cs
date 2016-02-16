using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Diff
{
  public interface IDiffRecord
  {
    DiffAction Action { get; }
    object Base { get; }
    object Compare { get; }
    IEnumerable<IDiffRecord> Changes { get; }
  }
  public interface IDiffRecord<T> : IDiffRecord
  {
    new T Base { get; }
    new T Compare { get; }
  }
}
