using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class JsonFormatter : IFormatProvider, ICustomFormatter
  {
    public string Format(string format, params object[] args)
    {
      return string.Format(this, format, args);
    }

    object IFormatProvider.GetFormat(Type formatType)
    {
      if (formatType == typeof(ICustomFormatter))
        return this;
      else
        return null;
    }

    string ICustomFormatter.Format(string format, object arg, IFormatProvider formatProvider)
    {
      if (arg == null) return null;
      if (!this.Equals(formatProvider)) return null;

      IFormattable formattable;
      if (ServerContext.TryCastNumber(arg, out formattable))
        return formattable.ToString(format, CultureInfo.CurrentCulture);
      
      formattable = arg as IFormattable;
      var value = formattable == null ?
        arg.ToString() :
        formattable.ToString(format, formatProvider);
      var builder = new StringBuilder(value.Length + 5).Append("\"");
      return AppendEscapedJson(builder, value).Append("\"").ToString();
    }

    public static StringBuilder AppendEscapedJson(StringBuilder builder, string value)
    {
      if (value == null) return builder;
      builder.EnsureCapacity(builder.Length + value.Length + 5);
      for (var i = 0; i < value.Length; i++)
      {
        switch (value[i])
        {
          case '\t':
            builder.Append(@"\t");
            break;
          case '\n':
            builder.Append(@"\n");
            break;
          case '\r':
            builder.Append(@"\r");
            break;
          case '"':
            builder.Append(@"\""");
            break;
          case '\\':
            builder.Append(@"\\");
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
