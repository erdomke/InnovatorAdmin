using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Json
{
  public static class Util
  {
    public static IJsonWriter Node(this IJsonWriter writer, IJsonNode node)
    {
      switch (node.Type)
      {
        case JsonNodeType.Array:
          if (!string.IsNullOrEmpty(node.Name)) writer.Prop(node.Name);
          writer.Array();
          break;
        case JsonNodeType.ArrayEnd:
          writer.ArrayEnd();
          break;
        case JsonNodeType.Object:
          if (!string.IsNullOrEmpty(node.Name)) writer.Prop(node.Name);
          writer.Object();
          break;
        case JsonNodeType.ObjectEnd:
          writer.ObjectEnd();
          break;
        case JsonNodeType.SimpleProperty:
          writer.Prop(node.Name, node.Value);
          break;
        default:
          writer.Value(node.Value);
          break;
      }
      return writer;
    }
    public static void WriteTo(this IEnumerable<IJsonNode> source, IJsonWriter writer)
    {
      foreach (var node in source)
      {
        writer.Node(node);
      }
    }
  }
}
