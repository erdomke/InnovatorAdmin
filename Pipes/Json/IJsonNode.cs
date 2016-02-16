using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Json
{
  public interface IJsonNode : Data.IFieldValue
  {
    JsonNodeType Type { get; }
  }
}
