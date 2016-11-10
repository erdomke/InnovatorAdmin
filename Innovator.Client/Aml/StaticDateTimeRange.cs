using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class StaticDateTimeRange
  {
    public DateTime? EndDate { get; set; }
    public DateTime? StartDate { get; set; }
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
      result.EndDate = EndDate.HasValue ? TimeZoneInfo.ConvertTime(EndDate.Value, this.TimeZone, timeZone) : (DateTime?)null;
      result.StartDate = StartDate.HasValue ? TimeZoneInfo.ConvertTime(StartDate.Value, this.TimeZone, timeZone) : (DateTime?)null;
      result.TimeZone = timeZone;
      return result;
    }
  }
}
