using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Data
{
  public interface IFieldValue
  {
    string Name { get; }
    object Value { get; }
  }
}
