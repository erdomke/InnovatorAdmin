using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Activity Template Assignment </summary>
  public class ActivityTemplateAssignment : Item
  {
    protected ActivityTemplateAssignment() { }
    public ActivityTemplateAssignment(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>escalate_to</c> property of the item</summary>
    public IProperty_Item EscalateTo()
    {
      return this.Property("escalate_to");
    }
    /// <summary>Retrieve the <c>for_all_members</c> property of the item</summary>
    public IProperty_Boolean ForAllMembers()
    {
      return this.Property("for_all_members");
    }
    /// <summary>Retrieve the <c>is_required</c> property of the item</summary>
    public IProperty_Boolean IsRequired()
    {
      return this.Property("is_required");
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