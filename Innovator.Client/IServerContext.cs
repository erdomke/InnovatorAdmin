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

    bool? AsBoolean(string value);
    DateTime? AsDateTime(string value);
    DateTime? AsDateTimeUtc(string value);
    decimal? AsDecimal(string value);
    double? AsDouble(string value);
    int? AsInt(string value);
    long? AsLong(string value);
    string Format(object value);
  }
}
