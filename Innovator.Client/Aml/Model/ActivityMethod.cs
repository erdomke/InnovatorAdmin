using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Activity Method </summary>
  public class ActivityMethod : Item, INullRelationship<Activity>, IRelationship<Method>
  {
    protected ActivityMethod() { }
    public ActivityMethod(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ActivityMethod() { Innovator.Client.Item.AddNullItem<ActivityMethod>(new ActivityMethod { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>event</c> property of the item</summary>
    public IProperty_Text Event()
    {
      return this.Property("event");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}