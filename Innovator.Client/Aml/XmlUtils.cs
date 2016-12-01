using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Innovator.Client
{
  internal static class XmlUtils
  {
    public static StringBuilder AppendEscapedXml(this StringBuilder builder, string value)
    {
      if (value == null) return builder;
      builder.EnsureCapacity(builder.Length + value.Length + 10);
      for (var i = 0; i < value.Length; i++)
      {
        switch (value[i])
        {
          case '&':
            builder.Append("&amp;");
            break;
          case '<':
            builder.Append("&lt;");
            break;
          case '>':
            builder.Append("&gt;");
            break;
          case '"':
            builder.Append("&quot;");
            break;
          case '\'':
            builder.Append("&apos;");
            break;
          default:
            builder.Append(value[i]);
            break;
        }
      }
      return builder;
    }
  }
}
