using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public static class UnixDate
  {
    private static DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime FromUnix(long seconds)
    {
      return _unixEpoch.AddSeconds(seconds);
    }
    public static long ToUnix(DateTime value)
    {
      var utc = value;
      if (value.Kind != DateTimeKind.Utc)
        utc = TimeZoneInfo.ConvertTimeToUtc(value, TimeZoneInfo.Local);
      return (long)Math.Floor((utc - _unixEpoch).TotalSeconds);
    }
  }
}
