#if !TIMEZONEINFO
using NodaTime;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client
{

  public class TimeZoneData : IEquatable<TimeZoneData>
  {
    public override bool Equals(object obj)
    {
      var tzd = obj as TimeZoneData;
      if (tzd == null)
        return false;
      return Equals(tzd);
    }

#if TIMEZONEINFO
    private TimeZoneInfo _timeZone;

    public string Id
    {
      get { return _timeZone.Id; }
    }

    public TimeSpan GetUtcOffset(DateTime dateTime)
    {
      return _timeZone.GetUtcOffset(dateTime);
    }

    public override int GetHashCode()
    {
      return _timeZone.GetHashCode();
    }
    public bool Equals(TimeZoneData other)
    {
      return _timeZone.Equals(other._timeZone);
    }

    public static DateTime ConvertTime(DateTime value, TimeZoneData from, TimeZoneData to)
    {
      return TimeZoneInfo.ConvertTime(value, from._timeZone, to._timeZone);
    }

    public static TimeZoneData ById(string value)
    {
      return new TimeZoneData() { _timeZone = TimeZoneInfo.FindSystemTimeZoneById(value) };
    }

    public static DateTimeOffset ConvertTime(DateTimeOffset value, TimeZoneData to)
    {
      return TimeZoneInfo.ConvertTime(value, to._timeZone);
    }

    private static readonly TimeZoneData _local = new TimeZoneData() { _timeZone = TimeZoneInfo.Local };
    private static readonly TimeZoneData _utc = new TimeZoneData() { _timeZone = TimeZoneInfo.Utc };
#else
    private DateTimeZone _timeZone;
    private string _id;

    public string Id
    {
      get { return _id; }
    }

    public static TimeZoneData ById(string value)
    {
      return new TimeZoneData()
      {
        _timeZone = DateTimeZoneProviders.Tzdb[WindowsTzToIana(value)],
        _id = value
      };
    }

    public TimeSpan GetUtcOffset(DateTime dateTime)
    {
      var zoned = LocalDateTime.FromDateTime(dateTime).InZoneLeniently(_timeZone);
      return TimeSpan.FromMilliseconds(_timeZone.GetUtcOffset(zoned.ToInstant()).Milliseconds);
    }

    public override int GetHashCode()
    {
      return _timeZone.GetHashCode() ^ _id.GetHashCode();
    }
    public bool Equals(TimeZoneData other)
    {
      return _timeZone.Equals(other._timeZone) && _id.Equals(other._id);
    }

    public static DateTime ConvertTime(DateTime value, TimeZoneData from, TimeZoneData to)
    {
      var zoned = LocalDateTime.FromDateTime(value).InZoneLeniently(from._timeZone);
      return zoned.WithZone(to._timeZone).ToDateTimeUnspecified();
    }

    public static DateTimeOffset ConvertTime(DateTimeOffset value, TimeZoneData to)
    {
      var zoned = ZonedDateTime.FromDateTimeOffset(value);
      return zoned.WithZone(to._timeZone).ToDateTimeOffset();
    }

    private static readonly TimeZoneData _local = TimeZoneData.ById(TimeZoneInfo.Local.StandardName);
    private static readonly TimeZoneData _utc = TimeZoneData.ById("UTC");

    private static string WindowsTzToIana(string value)
    {
      switch (value)
      {
        case "AUS Central Standard Time": return "Australia/Darwin";
        case "AUS Eastern Standard Time": return "Australia/Sydney";
        case "Afghanistan Standard Time": return "Asia/Kabul";
        case "Alaskan Standard Time": return "America/Anchorage";
        case "Aleutian Standard Time": return "America/Adak";
        case "Altai Standard Time": return "Asia/Barnaul";
        case "Arab Standard Time": return "Asia/Riyadh";
        case "Arabian Standard Time": return "Asia/Dubai";
        case "Arabic Standard Time": return "Asia/Baghdad";
        case "Argentina Standard Time": return "America/Buenos_Aires";
        case "Astrakhan Standard Time": return "Europe/Astrakhan";
        case "Atlantic Standard Time": return "America/Halifax";
        case "Aus Central W. Standard Time": return "Australia/Eucla";
        case "Azerbaijan Standard Time": return "Asia/Baku";
        case "Azores Standard Time": return "Atlantic/Azores";
        case "Bahia Standard Time": return "America/Bahia";
        case "Bangladesh Standard Time": return "Asia/Dhaka";
        case "Belarus Standard Time": return "Europe/Minsk";
        case "Bougainville Standard Time": return "Pacific/Bougainville";
        case "Canada Central Standard Time": return "America/Regina";
        case "Cape Verde Standard Time": return "Atlantic/Cape_Verde";
        case "Caucasus Standard Time": return "Asia/Yerevan";
        case "Cen. Australia Standard Time": return "Australia/Adelaide";
        case "Central America Standard Time": return "America/Guatemala";
        case "Central Asia Standard Time": return "Asia/Almaty";
        case "Central Brazilian Standard Time": return "America/Cuiaba";
        case "Central Europe Standard Time": return "Europe/Budapest";
        case "Central European Standard Time": return "Europe/Warsaw";
        case "Central Pacific Standard Time": return "Pacific/Guadalcanal";
        case "Central Standard Time (Mexico)": return "America/Mexico_City";
        case "Central Standard Time": return "America/Chicago";
        case "Chatham Islands Standard Time": return "Pacific/Chatham";
        case "China Standard Time": return "Asia/Shanghai";
        case "Cuba Standard Time": return "America/Havana";
        case "Dateline Standard Time": return "Etc/GMT+12";
        case "E. Africa Standard Time": return "Africa/Nairobi";
        case "E. Australia Standard Time": return "Australia/Brisbane";
        case "E. Europe Standard Time": return "Europe/Chisinau";
        case "E. South America Standard Time": return "America/Sao_Paulo";
        case "Easter Island Standard Time": return "Pacific/Easter";
        case "Eastern Standard Time (Mexico)": return "America/Cancun";
        case "Eastern Standard Time": return "America/New_York";
        case "Egypt Standard Time": return "Africa/Cairo";
        case "Ekaterinburg Standard Time": return "Asia/Yekaterinburg";
        case "FLE Standard Time": return "Europe/Kiev";
        case "Fiji Standard Time": return "Pacific/Fiji";
        case "GMT Standard Time": return "Europe/London";
        case "GTB Standard Time": return "Europe/Bucharest";
        case "Georgian Standard Time": return "Asia/Tbilisi";
        case "Greenland Standard Time": return "America/Godthab";
        case "Greenwich Standard Time": return "Atlantic/Reykjavik";
        case "Haiti Standard Time": return "America/Port-au-Prince";
        case "Hawaiian Standard Time": return "Pacific/Honolulu";
        case "India Standard Time": return "Asia/Calcutta";
        case "Iran Standard Time": return "Asia/Tehran";
        case "Israel Standard Time": return "Asia/Jerusalem";
        case "Jordan Standard Time": return "Asia/Amman";
        case "Kaliningrad Standard Time": return "Europe/Kaliningrad";
        case "Korea Standard Time": return "Asia/Seoul";
        case "Libya Standard Time": return "Africa/Tripoli";
        case "Line Islands Standard Time": return "Pacific/Kiritimati";
        case "Lord Howe Standard Time": return "Australia/Lord_Howe";
        case "Magadan Standard Time": return "Asia/Magadan";
        case "Marquesas Standard Time": return "Pacific/Marquesas";
        case "Mauritius Standard Time": return "Indian/Mauritius";
        case "Middle East Standard Time": return "Asia/Beirut";
        case "Montevideo Standard Time": return "America/Montevideo";
        case "Morocco Standard Time": return "Africa/Casablanca";
        case "Mountain Standard Time (Mexico)": return "America/Chihuahua";
        case "Mountain Standard Time": return "America/Denver";
        case "Myanmar Standard Time": return "Asia/Rangoon";
        case "N. Central Asia Standard Time": return "Asia/Novosibirsk";
        case "Namibia Standard Time": return "Africa/Windhoek";
        case "Nepal Standard Time": return "Asia/Katmandu";
        case "New Zealand Standard Time": return "Pacific/Auckland";
        case "Newfoundland Standard Time": return "America/St_Johns";
        case "Norfolk Standard Time": return "Pacific/Norfolk";
        case "North Asia East Standard Time": return "Asia/Irkutsk";
        case "North Asia Standard Time": return "Asia/Krasnoyarsk";
        case "North Korea Standard Time": return "Asia/Pyongyang";
        case "Omsk Standard Time": return "Asia/Omsk";
        case "Pacific SA Standard Time": return "America/Santiago";
        case "Pacific Standard Time (Mexico)": return "America/Tijuana";
        case "Pacific Standard Time": return "America/Los_Angeles";
        case "Pakistan Standard Time": return "Asia/Karachi";
        case "Paraguay Standard Time": return "America/Asuncion";
        case "Romance Standard Time": return "Europe/Paris";
        case "Russia Time Zone 10": return "Asia/Srednekolymsk";
        case "Russia Time Zone 11": return "Asia/Kamchatka";
        case "Russia Time Zone 3": return "Europe/Samara";
        case "Russian Standard Time": return "Europe/Moscow";
        case "SA Eastern Standard Time": return "America/Cayenne";
        case "SA Pacific Standard Time": return "America/Bogota";
        case "SA Western Standard Time": return "America/La_Paz";
        case "SE Asia Standard Time": return "Asia/Bangkok";
        case "Saint Pierre Standard Time": return "America/Miquelon";
        case "Sakhalin Standard Time": return "Asia/Sakhalin";
        case "Samoa Standard Time": return "Pacific/Apia";
        case "Singapore Standard Time": return "Asia/Singapore";
        case "South Africa Standard Time": return "Africa/Johannesburg";
        case "Sri Lanka Standard Time": return "Asia/Colombo";
        case "Syria Standard Time": return "Asia/Damascus";
        case "Taipei Standard Time": return "Asia/Taipei";
        case "Tasmania Standard Time": return "Australia/Hobart";
        case "Tocantins Standard Time": return "America/Araguaina";
        case "Tokyo Standard Time": return "Asia/Tokyo";
        case "Tomsk Standard Time": return "Asia/Tomsk";
        case "Tonga Standard Time": return "Pacific/Tongatapu";
        case "Transbaikal Standard Time": return "Asia/Chita";
        case "Turkey Standard Time": return "Europe/Istanbul";
        case "Turks And Caicos Standard Time": return "America/Grand_Turk";
        case "US Eastern Standard Time": return "America/Indianapolis";
        case "US Mountain Standard Time": return "America/Phoenix";
        case "UTC": return "Etc/UTC";
        case "UTC+12": return "Etc/GMT-12";
        case "UTC-02": return "Etc/GMT+2";
        case "UTC-08": return "Etc/GMT+8";
        case "UTC-09": return "Etc/GMT+9";
        case "UTC-11": return "Etc/GMT+11";
        case "Ulaanbaatar Standard Time": return "Asia/Ulaanbaatar";
        case "Venezuela Standard Time": return "America/Caracas";
        case "Vladivostok Standard Time": return "Asia/Vladivostok";
        case "W. Australia Standard Time": return "Australia/Perth";
        case "W. Central Africa Standard Time": return "Africa/Lagos";
        case "W. Europe Standard Time": return "Europe/Berlin";
        case "W. Mongolia Standard Time": return "Asia/Hovd";
        case "West Asia Standard Time": return "Asia/Tashkent";
        case "West Bank Standard Time": return "Asia/Hebron";
        case "West Pacific Standard Time": return "Pacific/Port_Moresby";
        case "Yakutsk Standard Time": return "Asia/Yakutsk";
      }
      throw new ArgumentException();
    }
#endif

    public static TimeZoneData Local { get { return _local; } }
    public static TimeZoneData Utc { get { return _utc; } }
  }
}
