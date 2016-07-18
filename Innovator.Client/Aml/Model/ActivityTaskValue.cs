using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Activity Task Value </summary>
  public class ActivityTaskValue : Item
  {
    protected ActivityTaskValue() { }
    public ActivityTaskValue(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>comments</c> property of the item</summary>
    public IProperty_Text Comments()
    {
      return this.Property("comments");
    }
    /// <summary>Retrieve the <c>completed_by</c> property of the item</summary>
    public IProperty_Item CompletedBy()
    {
      return this.Property("completed_by");
    }
    /// <summary>Retrieve the <c>completed_on</c> property of the item</summary>
    public IProperty_Date CompletedOn()
    {
      return this.Property("completed_on");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>task</c> property of the item</summary>
    public IProperty_Item Task()
    {
      return this.Property("task");
    }
  }
}