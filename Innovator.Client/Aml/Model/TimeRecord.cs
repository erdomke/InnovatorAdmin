using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Time Record </summary>
  public class TimeRecord : Item
  {
    protected TimeRecord() { }
    public TimeRecord(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>date_from</c> property of the item</summary>
    public IProperty_Date DateFrom()
    {
      return this.Property("date_from");
    }
    /// <summary>Retrieve the <c>date_to</c> property of the item</summary>
    public IProperty_Date DateTo()
    {
      return this.Property("date_to");
    }
    /// <summary>Retrieve the <c>notes</c> property of the item</summary>
    public IProperty_Text Notes()
    {
      return this.Property("notes");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>work_hours</c> property of the item</summary>
    public IProperty_Number WorkHours()
    {
      return this.Property("work_hours");
    }
    /// <summary>Retrieve the <c>work_identity</c> property of the item</summary>
    public IProperty_Item WorkIdentity()
    {
      return this.Property("work_identity");
    }
  }
}