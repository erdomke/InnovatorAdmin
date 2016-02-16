using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  public interface IDataRecord : IEnumerable<IFieldValue>
  {
    int FieldCount { get; }
    object Item(string name);
    FieldStatus Status(string name);
  }
}
