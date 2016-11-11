using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Business Calendar Year </summary>
  public class BusinessCalendarYear : Item
  {
    protected BusinessCalendarYear() { }
    public BusinessCalendarYear(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static BusinessCalendarYear() { Innovator.Client.Item.AddNullItem<BusinessCalendarYear>(new BusinessCalendarYear { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>weekend_days_off</c> property of the item</summary>
    public IProperty_Boolean WeekendDaysOff()
    {
      return this.Property("weekend_days_off");
    }
    /// <summary>Retrieve the <c>year</c> property of the item</summary>
    public IProperty_Number Year()
    {
      return this.Property("year");
    }
  }
}