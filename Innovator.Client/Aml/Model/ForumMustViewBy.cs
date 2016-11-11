using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ForumMustViewBy </summary>
  public class ForumMustViewBy : Item, INullRelationship<Forum>
  {
    protected ForumMustViewBy() { }
    public ForumMustViewBy(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ForumMustViewBy() { Innovator.Client.Item.AddNullItem<ForumMustViewBy>(new ForumMustViewBy { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>must_view_id</c> property of the item</summary>
    public IProperty_Item<Identity> MustViewId()
    {
      return this.Property("must_view_id");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}