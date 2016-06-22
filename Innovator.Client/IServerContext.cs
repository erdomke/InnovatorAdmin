using System;
using System.Runtime.Serialization;

namespace Innovator.Client
{
  public interface IServerContext : ISerializable, IFormatProvider, ICustomFormatter
  {
    string DefaultLanguageCode { get; }
    string DefaultLanguageSuffix { get; }
    string LanguageCode { get; }
    string LanguageSuffix { get; }
    string Locale { get; }
    string TimeZone { get; }

    bool? AsBoolean(object value);
    DateTime? AsDateTime(object value);
    DateTime? AsDateTimeUtc(object value);
    decimal? AsDecimal(object value);
    double? AsDouble(object value);
    int? AsInt(object value);
    long? AsLong(object value);
    string Format(object value);
  }
}
