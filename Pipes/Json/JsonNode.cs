using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Json
{
  public class JsonNode : IJsonNode
  {
    public JsonNodeType Type { get; set; }
    public string Name { get; set; }
    public object Value { get; set; }
  }
}
