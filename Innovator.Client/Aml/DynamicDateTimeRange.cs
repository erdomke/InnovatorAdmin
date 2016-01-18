using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class DynamicDateTimeRange
  {
    public DateMagnitude EndMagnitude { get; set; }
    public int EndOffset { get; set; }
    public DayOfWeek FirstDayOfWeek { get; set; }
    public DateMagnitude StartMagnitude { get; set; }
    public int StartOffset { get; set; }

    public DynamicDateTimeRange()
    {
      this.EndMagnitude = DateMagnitude.Year;
      this.EndOffset = 1000;
      this.FirstDayOfWeek = DayOfWeek.Sunday;
      this.StartMagnitude = DateMagnitude.Year;
      this.StartOffset = -1000;
    }

    public string Serialize()
    {
      return this.StartMagnitude.ToString() + "|" + this.StartOffset.ToString() + "|"
        + this.EndMagnitude.ToString() + "|" + this.EndOffset.ToString() + "|"
        + this.FirstDayOfWeek.ToString();
    }

    public StaticDateTimeRange ToStatic(TimeZoneInfo timeZone)
    {
      var result = new StaticDateTimeRange();
      result.StartDate = GetDateFromDynamic(this.StartMagnitude, this.StartOffset, false, DateTimeOffset.UtcNow, this.FirstDayOfWeek, timeZone);
      result.EndDate = GetDateFromDynamic(this.EndMagnitude, this.EndOffset, true, DateTimeOffset.UtcNow, this.FirstDayOfWeek, timeZone);
      result.TimeZone = timeZone;
      return result;
    }

    public static DynamicDateTimeRange Deserialize(string value)
    {
      var parts = value.Split('|');
      if (parts.Length != 5) throw new ArgumentException();

      var result = new DynamicDateTimeRange();
      result.StartMagnitude = (DateMagnitude)Enum.Parse(typeof(DateMagnitude), parts[0]);
      result.StartOffset = int.Parse(parts[1]);
      result.EndMagnitude = (DateMagnitude)Enum.Parse(typeof(DateMagnitude), parts[2]);
      result.EndOffset = int.Parse(parts[3]);
      result.FirstDayOfWeek = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), parts[4]);
      return result;
    }

    public static DateTime GetDateFromDynamic(DateMagnitude magnitude, int offset, bool isEndDate, DateTimeOffset todaysDate, DayOfWeek weekStart, TimeZoneInfo timeZone)
    {
      var localToday = TimeZoneInfo.ConvertTime(todaysDate, timeZone);
      DateTimeOffset result;

      if (isEndDate) offset++;
      switch (magnitude)
      {
        case DateMagnitude.BusinessDay:
          var offsetMagn = Math.Abs(offset);
          var offsetSign = Math.Sign(offset);
          var i = 0;
          result = localToday;
          while (i < offsetMagn)
          {
            result = result.AddDays(offsetSign);
            if (result.DayOfWeek != DayOfWeek.Sunday && result.DayOfWeek != DayOfWeek.Saturday)
              i++;
          }
          break;
        case DateMagnitude.Week:
          result = GetWeekStart(todaysDate, weekStart).AddDays(offset * 7);
          break;
        case DateMagnitude.Month:
          result = new DateTimeOffset(todaysDate.Year, todaysDate.Month, 1, 0, 0, 0, localToday.Offset).AddMonths(offset);
          break;
        case DateMagnitude.Quarter:
          switch (todaysDate.Month)
          {
            case 1:
            case 2:
            case 3:
              result = new DateTimeOffset(todaysDate.Year, 1, 1, 0, 0, 0, localToday.Offset).AddMonths(offset * 3);
              break;
            case 4:
            case 5:
            case 6:
              result = new DateTimeOffset(todaysDate.Year, 4, 1, 0, 0, 0, localToday.Offset).AddMonths(offset * 3);
              break;
            case 7:
            case 8:
            case 9:
              result = new DateTimeOffset(todaysDate.Year, 7, 1, 0, 0, 0, localToday.Offset).AddMonths(offset * 3);
              break;
            default:
              result = new DateTimeOffset(todaysDate.Year, 10, 1, 0, 0, 0, localToday.Offset).AddMonths(offset * 3);
              break;
          }
          break;
        case DateMagnitude.Year:
          result = new DateTimeOffset(todaysDate.Year, 1, 1, 0, 0, 0, localToday.Offset).AddYears(offset * 3);
          break;
        default:
          result = todaysDate.AddDays(offset);
          break;
      }

      if (isEndDate) return result.Date.AddMilliseconds(-1);
      return result.Date;
    }
    public static DateTimeOffset GetWeekStart(DateTimeOffset value, DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
    {
      var offset = (int)firstDayOfWeek - (int)value.DayOfWeek;
      if (offset > 0) offset -= 7;
      return value.AddDays(offset);
    }
  }
}
