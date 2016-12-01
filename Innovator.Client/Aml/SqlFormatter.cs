using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class SqlFormatter : IFormatProvider, ICustomFormatter
  {
    private IServerContext _context;

    public SqlFormatter(IServerContext context)
    {
      _context = context;
    }

    public object GetFormat(Type formatType)
    {
      if (formatType == typeof(ICustomFormatter))
        return this;
      else
        return null;
    }

    public string Format(string format, object arg, IFormatProvider formatProvider)
    {
      if (!this.Equals(formatProvider)) return null;
      if (arg == null) return "null";

      var parts = (format ?? "").Split(':');
      string formatString;
      var numberMode = false;
      IFormattable number = null;

      switch (parts[0])
      {
        case "str":
          numberMode = false;
          formatString = string.Join(":", parts, 1, parts.Length - 1);
          break;
        case "num":
          if (!ServerContext.TryCastNumber(arg, out number))
          {
            number = double.Parse(arg.ToString());
            numberMode = true;
          }
          formatString = string.Join(":", parts, 1, parts.Length - 1);
          break;
        default:
          numberMode = ServerContext.TryCastNumber(arg, out number);
          formatString = format;
          break;
      }

      if (numberMode)
      {
        return number.ToString(formatString, CultureInfo.InvariantCulture);
      }
      else
      {
        return "N'" + Render(arg,
          n => n.ToString(formatString, CultureInfo.InvariantCulture),
          s => SqlEscape(arg, formatString)) + "'";
      }

    }

    private static string SqlEscape(object obj, string format)
    {
      if (obj == null) return null;

      var value = ServerContext.GetString(obj, format);
      var builder = new StringBuilder(value.Length + 10);
      for (var i = 0; i < value.Length; i++)
      {
        switch (value[i])
        {
          case '\'':
            builder.Append("''");
            break;
          default:
            builder.Append(value[i]);
            break;
        }
      }
      return builder.ToString();
    }

    internal string Format(object arg)
    {
      return Render(arg,
          n => n.ToString(null, CultureInfo.InvariantCulture),
          s => SqlEscape(arg, null));
    }

    private string Render(object arg, Func<IFormattable, string> numberRenderer, Func<object, string> stringRenderer)
    {
      IFormattable number;
      if (arg == null)
      {
        return "null";
      }
      else if (arg is DateTime)
      {
        return Render((DateTime)arg);
      }
      else if (arg is bool || arg is Guid)
      {
        return _context.Format(arg);
      }
      else if (arg is Condition)
      {
        switch ((Condition)arg)
        {
          case Condition.Between:
            return "between";
          case Condition.Equal:
            return "=";
          case Condition.GreaterThan:
            return ">";
          case Condition.GreaterThanEqual:
            return ">=";
          case Condition.In:
            return "in";
          case Condition.Is:
            return "is";
          case Condition.IsNotNull:
            return "is not null";
          case Condition.IsNull:
            return "is null";
          case Condition.LessThan:
            return "<";
          case Condition.LessThanEqual:
            return "<=";
          case Condition.Like:
            return "like";
          case Condition.NotBetween:
            return "not between";
          case Condition.NotEqual:
            return "<>";
          case Condition.NotIn:
            return "not in";
          case Condition.NotLike:
            return "not like";
          default:
            throw new NotImplementedException();
        }
      }
      else if (ServerContext.TryCastNumber(arg, out number))
      {
        return numberRenderer(number);
      }
      else
      {
        return stringRenderer(arg.ToString());
      }
    }

    private string Render(DateTime value)
    {
      var converted = value;
      if (value.Kind != DateTimeKind.Utc)
      {
        converted = TimeZoneData.ConvertTime(value, TimeZoneData.Local, TimeZoneData.Utc);
      }
      return converted.ToString("s");
    }
  }
}
