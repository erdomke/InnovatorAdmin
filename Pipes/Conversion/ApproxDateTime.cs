using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pipes.Conversion
{
  public class ApproxDateTime
  {
    private int _century = 0;
    private int _decade = 0;
    private int _yearNum = 1;
    private int _month = 1;
    private int _day = 1;
    private int _hour = 0;
    private int _minute = 0;
    private int _second = 0;
    private int _millisecond = 0;
    private Certainty _timeCertainty;

    public int Century 
    { 
      get { return _century; }
      set { _century = AssertRange(value, "Century", 0, 99); this.CenturyCertainty &= ~Certainty.Unspecified; }
    }
    public Certainty CenturyCertainty { get; set; }
    public int Year
    {
      get
      {
        return this.Century * 100 + this.Decade * 10 + this.YearNum;
      }
      set
      {
        this.Century = value / 100;
        this.Decade = value / 10 % 10;
        this.YearNum = value % 10;
      }
    }
    public int Decade
    {
      get { return _decade; }
      set { _decade = AssertRange(value, "Decade", 0, 9); this.DecadeCertainty &= ~Certainty.Unspecified; }
    }
    public Certainty DecadeCertainty { get; set; }
    public int YearNum
    {
      get { return _yearNum; }
      set { _yearNum = AssertRange(value, "Year Number", 0, 9); this.YearNumCertainty &= ~Certainty.Unspecified; }
    }
    public Certainty YearNumCertainty { get; set; }
    public int Month
    {
      get { return _month; }
      set { _month = AssertRange(value, "Month", 1, 12); this.MonthCertainty &= ~Certainty.Unspecified; }
    }
    public Certainty MonthCertainty { get; set; }
    public int Day
    {
      get { return _day; }
      set { _day = AssertRange(value, "Day", 1, 31); this.DayCertainty &= ~Certainty.Unspecified; }
    }
    public Certainty DayCertainty { get; set; }
    public int Hour
    {
      get { return _hour; }
      set { _hour = AssertRange(value, "Hour", 0, 23); _timeCertainty &= ~Certainty.Unspecified; }
    }
    public int Minute
    {
      get { return _minute; }
      set { _minute = AssertRange(value, "Minute", 0, 59); _timeCertainty &= ~Certainty.Unspecified; }
    }
    public int Second
    {
      get { return _second; }
      set { _second = AssertRange(value, "Second", 0, 59); _timeCertainty &= ~Certainty.Unspecified; }
    }
    public int Millisecond
    {
      get { return _millisecond; }
      set { _millisecond = AssertRange(value, "Millisecond", 0, 999); _timeCertainty &= ~Certainty.Unspecified; }
    }
    public Certainty DateCertainty
    {
      get
      {
        return this.CenturyCertainty & this.DayCertainty & this.DecadeCertainty & this.MonthCertainty & this.YearNumCertainty;
      }
      set
      {
        this.CenturyCertainty = value;
        this.DayCertainty = value;
        this.DecadeCertainty = value;
        this.MonthCertainty = value;
        this.YearNumCertainty = value;
      }
    }
    public Certainty TimeCertainty
    {
      get
      {
        return _timeCertainty;
      }
    }

    public ApproxDateTime()
    {
      _timeCertainty = Certainty.Unspecified;
      this.CenturyCertainty = Certainty.Unspecified;
      this.DayCertainty = Certainty.Unspecified;
      this.DecadeCertainty = Certainty.Unspecified;
      this.MonthCertainty = Certainty.Unspecified;
      this.YearNumCertainty = Certainty.Unspecified;
    }


    private int AssertRange(int value, string name, int min, int max)
    {
      if (value < min || value > max) throw new ArgumentOutOfRangeException(string.Format("{0} should be between {1} and {2}", name, min, max));
      return value;
    }

    /// <summary>
    /// http://www.loc.gov/standards/datetime/pre-submission.html
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      var builder = new StringBuilder();
      builder.Append(_century.ToString("D2"));
      switch (this.DecadeCertainty)
      {
        case Certainty.Unknowable:
          builder.Append('x');
          break;
        case Certainty.Unspecified:
          builder.Append('u');
          break;
        default:
          builder.Append(_decade);
          break;
      }
      switch (this.YearNumCertainty)
      {
        case Certainty.Unknowable:
          builder.Append('x');
          break;
        case Certainty.Unspecified:
          builder.Append('u');
          break;
        default:
          builder.Append(_yearNum);
          break;
      }
      builder.Append('-');
      if (this.MonthCertainty == Certainty.Unspecified)
      {
        builder.Append("uu");
      }
      else
      {
        builder.Append(_month.ToString("D2"));
      }
      builder.Append('-');
      if (this.DayCertainty == Certainty.Unspecified)
      {
        builder.Append("uu");
      }
      else
      {
        builder.Append(_day.ToString("D2"));
      }
      builder.Append('T');
      builder.Append(_hour.ToString("D2"));
      builder.Append(':');
      builder.Append(_minute.ToString("D2"));
      builder.Append(':');
      builder.Append(_second.ToString("D2"));
      builder.Append('.');
      builder.Append(_millisecond.ToString("D3"));
      return builder.ToString();
    }
  }
}
