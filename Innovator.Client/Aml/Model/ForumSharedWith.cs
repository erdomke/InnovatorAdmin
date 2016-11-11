using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ForumSharedWith </summary>
  public class ForumSharedWith : Item, INullRelationship<Forum>
  {
    protected ForumSharedWith() { }
    public ForumSharedWith(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ForumSharedWith() { Innovator.Client.Item.AddNullItem<ForumSharedWith>(new ForumSharedWith { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>shared_with_id</c> property of the item</summary>
    public IProperty_Item<Identity> SharedWithId()
    {
      return this.Property("shared_with_id");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}