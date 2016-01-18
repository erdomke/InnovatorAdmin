using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class StaticDateTimeRange
  {
    public DateTime EndDate { get; set; }
    public DateTime StartDate { get; set; }
    public TimeZoneInfo TimeZone { get; set; }

    public StaticDateTimeRange()
    {
      this.EndDate = DateTime.Now;
      this.StartDate = this.EndDate;
      this.TimeZone = TimeZoneInfo.Local;
    }

    public StaticDateTimeRange ToTimeZone(TimeZoneInfo timeZone)
    {
      if (timeZone == this.TimeZone) return this;
      var result = new StaticDateTimeRange();
      result.EndDate = TimeZoneInfo.ConvertTime(this.EndDate, this.TimeZone, timeZone);
      result.StartDate = TimeZoneInfo.ConvertTime(this.StartDate, this.TimeZone, timeZone);
      result.TimeZone = timeZone;
      return result;
    }
  }
}
