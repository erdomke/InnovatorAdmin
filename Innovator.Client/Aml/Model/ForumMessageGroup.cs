using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ForumMessageGroup </summary>
  public class ForumMessageGroup : Item
  {
    protected ForumMessageGroup() { }
    public ForumMessageGroup(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>aml</c> property of the item</summary>
    public IProperty_Text Aml()
    {
      return this.Property("aml");
    }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>group_type</c> property of the item</summary>
    public IProperty_Text GroupType()
    {
      return this.Property("group_type");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>user_criteria_id</c> property of the item</summary>
    public IProperty_Text UserCriteriaId()
    {
      return this.Property("user_criteria_id");
    }
  }
}