using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Activity Transition </summary>
  public class ActivityTransition : Item, INullRelationship<Activity>, IRelationship<LifeCycleTransition>
  {
    protected ActivityTransition() { }
    public ActivityTransition(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ActivityTransition() { Innovator.Client.Item.AddNullItem<ActivityTransition>(new ActivityTransition { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>controlled_itemtype</c> property of the item</summary>
    public IProperty_Item<ItemType> ControlledItemtype()
    {
      return this.Property("controlled_itemtype");
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