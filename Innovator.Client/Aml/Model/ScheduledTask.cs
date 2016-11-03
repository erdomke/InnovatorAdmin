using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Scheduled Task </summary>
  public class ScheduledTask : Item
  {
    protected ScheduledTask() { }
    public ScheduledTask(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ScheduledTask() { Innovator.Client.Item.AddNullItem<ScheduledTask>(new ScheduledTask { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>day_of_week</c> property of the item</summary>
    public IProperty_Text DayOfWeek()
    {
      return this.Property("day_of_week");
    }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>hour</c> property of the item</summary>
    public IProperty_Number Hour()
    {
      return this.Property("hour");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>run_as</c> property of the item</summary>
    public IProperty_Item<User> RunAs()
    {
      return this.Property("run_as");
    }
    /// <summary>Retrieve the <c>run_it</c> property of the item</summary>
    public IProperty_Text RunIt()
    {
      return this.Property("run_it");
    }
    /// <summary>Retrieve the <c>start_date</c> property of the item</summary>
    public IProperty_Date StartDate()
    {
      return this.Property("start_date");
    }
    /// <summary>Retrieve the <c>task</c> property of the item</summary>
    public IProperty_Item<Method> Task()
    {
      return this.Property("task");
    }
    /// <summary>Retrieve the <c>weekdays</c> property of the item</summary>
    public IProperty_Boolean Weekdays()
    {
      return this.Property("weekdays");
    }
  }
}