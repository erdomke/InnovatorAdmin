using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Innovator.Client
{
  [Serializable]
  public sealed class ServerContext : IServerContext
  {
    private CultureInfo _culture;
    private TimeZoneInfo _timeZone;

    public string DefaultLanguageCode { get; set; }
    public string DefaultLanguageSuffix { get; set; }
    public string LanguageCode { get; set; }
    public string LanguageSuffix { get; set; }
    public string Locale
    {
      get { return _culture.Name; }
      set { _culture = CultureInfo.GetCultureInfo(value ?? ""); }
    }
    public string TimeZone
    {
      get { return _timeZone.Id; }
      set { _timeZone = TimeZoneInfo.FindSystemTimeZoneById(value); }
    }

    public ServerContext()
    {
      _timeZone = TimeZoneInfo.Local;
      _culture = CultureInfo.InvariantCulture;
      this.LanguageCode = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
    }
    public ServerContext(string timeZoneName)
    {
      this.TimeZone = timeZoneName;
      _culture = CultureInfo.InvariantCulture;
    }
    public ServerContext(SerializationInfo info, StreamingContext context)
    {
      this.DefaultLanguageCode = info.GetString("default_language_code");
      this.DefaultLanguageSuffix = info.GetString("default_language_suffix");
      this.LanguageCode = info.GetString("language_code");
      this.LanguageSuffix = info.GetString("language_suffix");
      this.Locale = info.GetString("locale");
      this.TimeZone = info.GetString("time_zone");
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("default_language_code", this.DefaultLanguageCode);
      info.AddValue("default_language_suffix", this.DefaultLanguageSuffix);
      info.AddValue("language_code", this.LanguageCode);
      info.AddValue("language_suffix", this.LanguageSuffix);
      info.AddValue("locale", this.Locale);
      info.AddValue("time_zone", this.TimeZone);
    }

    public bool? AsBoolean(string value)
    {
      if (string.IsNullOrEmpty(value)) return null;
      if (value == "0")
      {
        return false;
      }
      else if (value == "1")
      {
        return true;
      }
      else
      {
        throw new InvalidCastException();
      }
    }
    public DateTime? AsDateTime(string value)
    {
      if (string.IsNullOrEmpty(value)) return null;
      var result = DateTime.Parse(value, _culture, DateTimeStyles.AssumeLocal);
      if (_timeZone.Equals(TimeZoneInfo.Local)) return result;
      result = DateTime.SpecifyKind(result, DateTimeKind.Unspecified);
      result = TimeZoneInfo.ConvertTime(result, _timeZone, TimeZoneInfo.Local);
      return result;
    }
    public DateTime? AsDateTimeUtc(string value)
    {
      if (string.IsNullOrEmpty(value)) return null;
      var result = DateTime.Parse(value, _culture, DateTimeStyles.AssumeUniversal);
      if (_timeZone == TimeZoneInfo.Utc) return result;
      result = DateTime.SpecifyKind(result, DateTimeKind.Unspecified);
      result = TimeZoneInfo.ConvertTime(result, _timeZone, TimeZoneInfo.Utc);
      return result;
    }
    public decimal? AsDecimal(string value)
    {
      if (string.IsNullOrEmpty(value)) return null;
      return decimal.Parse(value, CultureInfo.InvariantCulture);
    }
    public double? AsDouble(string value)
    {
      if (string.IsNullOrEmpty(value)) return null;
      return double.Parse(value, CultureInfo.InvariantCulture);
    }
    public int? AsInt(string value)
    {
      if (string.IsNullOrEmpty(value)) return null;
      return int.Parse(value, CultureInfo.InvariantCulture);
    }
    public long? AsLong(string value)
    {
      if (string.IsNullOrEmpty(value)) return null;
      return long.Parse(value, CultureInfo.InvariantCulture);
    }

    public string Format(object value)
    {
      return Format(value, n => n.ToString(null, CultureInfo.InvariantCulture), s => s.ToString());
    }
    private string Format(object value, Func<IFormattable, string> numberRenderer, Func<object, string> stringRenderer)
    {
      IFormattable number;
      if (value == null)
      {
        return null;
      }
      else if (value is bool)
      {
        if ((bool)value)
        {
          return "1";
        }
        else
        {
          return "0";
        }
      }
      else if (value is Condition)
      {
        switch ((Condition)value)
        {
          case Condition.Between:
            return "between";
          case Condition.Equal:
            return "eq";
          case Condition.GreaterThan:
            return "gt";
          case Condition.GreaterThanEqual:
            return "ge";
          case Condition.In:
            return "in";
          case Condition.Is:
            return "is";
          case Condition.IsNotNull:
            return "is not null";
          case Condition.IsNull:
            return "is null";
          case Condition.LessThan:
            return "lt";
          case Condition.LessThanEqual:
            return "le";
          case Condition.Like:
            return "like";
          case Condition.NotBetween:
            return "not between";
          case Condition.NotEqual:
            return "ne";
          case Condition.NotIn:
            return "not in";
          case Condition.NotLike:
            return "not like";
          default:
            throw new NotImplementedException();
        }
      }
      else if (TryCastNumber(value, out number))
      {
        return numberRenderer(number);
      }
      else if (value is Guid)
      {
        return ((Guid)value).ToString("N").ToUpperInvariant();
      }
      else if (value is DateTime)
      {
        return Render((DateTime)value);
      }
      else if (value is StaticDateTimeRange)
      {
        var range = ((StaticDateTimeRange)value).ToTimeZone(_timeZone);
        return stringRenderer(range.StartDate.ToString("s") + " AND " + range.EndDate.ToString("s"));
      }
      else if (value is DynamicDateTimeRange)
      {
        var range = ((DynamicDateTimeRange)value).ToStatic(_timeZone);
        return stringRenderer(range.StartDate.ToString("s") + " AND " + range.EndDate.ToString("s"));
      }
      else
      {
        return stringRenderer(value.ToString());
      }
    }

    private string Render(DateTime value)
    {
      var converted = value;
      if (value.Kind == DateTimeKind.Utc)
      {
        if (_timeZone != TimeZoneInfo.Utc)
        {
          converted = TimeZoneInfo.ConvertTime(value, TimeZoneInfo.Utc, _timeZone);
        }
      }
      else if (_timeZone != TimeZoneInfo.Local) // Assume local
      {
        converted = TimeZoneInfo.ConvertTime(value, TimeZoneInfo.Local, _timeZone);
      }
      if (converted < SqlDateTime.MinValue.Value
        || converted > SqlDateTime.MaxValue.Value) return null;
      return converted.ToString("s");
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
      // Check whether this is an appropriate callback              
      if (!this.Equals(formatProvider)) return null;
      if (arg == null) return null;

      var parts = (format ?? "").Split(':');
      var sql = new SqlFormatter(this);
      string formatString;

      switch (parts[0])
      {
        case "aml":
          formatString = string.Join(":", parts, 1, parts.Length - 1);
          return Format(arg,
            n => n.ToString(format, CultureInfo.InvariantCulture),
            o => GetString(o, formatString));
        case "sql":
          formatString = string.Join(":", parts, 1, parts.Length - 1);
          return XmlEscape(sql.Format(formatString, arg, sql));
        case "rawsql":
          formatString = string.Join(":", parts, 1, parts.Length - 1);
          return sql.Format(formatString, arg, sql);
        default:
          formatString = format;
          return Format(arg,
            n => n.ToString(format, CultureInfo.InvariantCulture),
            o => XmlEscape(GetString(o, formatString)));
      }
    }

    internal static string GetString(object obj, string format)
    {
      var formattable = obj as IFormattable;
      if (!string.IsNullOrEmpty(format) && formattable != null)
        return formattable.ToString(format, CultureInfo.InvariantCulture);
      
      var node = obj as System.Xml.XmlNode;
      if (node != null) return node.OuterXml;

      return obj.ToString();
    }

    internal static string XmlEscape(string value)
    {
      return new StringBuilder(value.Length + 10).AppendEscapedXml(value).ToString();
    }
    
    internal static bool TryCastNumber(object value, out IFormattable number)
    {
      if (value is short || value is int || value is long
        || value is ushort || value is uint || value is ulong
        || value is byte)
      {
        number = (IFormattable)value;
        return true;
      }
      else if (value is float || value is double || value is decimal)
      {
        number = Convert.ToDecimal(value);
        return true;
      }
      number = null;
      return false;
    }
  }
}
