using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Json
{
  public enum JsonNodeType
  {
    Array,
    ArrayEnd,
    Object,
    ObjectEnd,
    SimpleProperty,
    Value
  }
}
