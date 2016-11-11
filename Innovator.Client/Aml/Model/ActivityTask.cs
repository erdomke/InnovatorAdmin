using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Activity Task </summary>
  public class ActivityTask : Item, INullRelationship<Activity>
  {
    protected ActivityTask() { }
    public ActivityTask(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ActivityTask() { Innovator.Client.Item.AddNullItem<ActivityTask>(new ActivityTask { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>is_required</c> property of the item</summary>
    public IProperty_Boolean IsRequired()
    {
      return this.Property("is_required");
    }
    /// <summary>Retrieve the <c>sequence</c> property of the item</summary>
    public IProperty_Number Sequence()
    {
      return this.Property("sequence");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}