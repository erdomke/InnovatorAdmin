using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Activity Assignment </summary>
  public class ActivityAssignment : Item, INullRelationship<Activity>, IRelationship<Identity>
  {
    protected ActivityAssignment() { }
    public ActivityAssignment(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ActivityAssignment() { Innovator.Client.Item.AddNullItem<ActivityAssignment>(new ActivityAssignment { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>claimed_by</c> property of the item</summary>
    public IProperty_Item<Identity> ClaimedBy()
    {
      return this.Property("claimed_by");
    }
    /// <summary>Retrieve the <c>closed_by</c> property of the item</summary>
    public IProperty_Item<User> ClosedBy()
    {
      return this.Property("closed_by");
    }
    /// <summary>Retrieve the <c>closed_on</c> property of the item</summary>
    public IProperty_Date ClosedOn()
    {
      return this.Property("closed_on");
    }
    /// <summary>Retrieve the <c>comments</c> property of the item</summary>
    public IProperty_Text Comments()
    {
      return this.Property("comments");
    }
    /// <summary>Retrieve the <c>escalate_to</c> property of the item</summary>
    public IProperty_Item<Identity> EscalateTo()
    {
      return this.Property("escalate_to");
    }
    /// <summary>Retrieve the <c>for_all_members</c> property of the item</summary>
    public IProperty_Boolean ForAllMembers()
    {
      return this.Property("for_all_members");
    }
    /// <summary>Retrieve the <c>is_disabled</c> property of the item</summary>
    public IProperty_Boolean IsDisabled()
    {
      return this.Property("is_disabled");
    }
    /// <summary>Retrieve the <c>is_overdue</c> property of the item</summary>
    public IProperty_Boolean IsOverdue()
    {
      return this.Property("is_overdue");
    }
    /// <summary>Retrieve the <c>is_required</c> property of the item</summary>
    public IProperty_Boolean IsRequired()
    {
      return this.Property("is_required");
    }
    /// <summary>Retrieve the <c>path</c> property of the item</summary>
    public IProperty_Text Path()
    {
      return this.Property("path");
    }
    /// <summary>Retrieve the <c>reminders_sent</c> property of the item</summary>
    public IProperty_Number RemindersSent()
    {
      return this.Property("reminders_sent");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>voting_weight</c> property of the item</summary>
    public IProperty_Number VotingWeight()
    {
      return this.Property("voting_weight");
    }
  }
}