using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ForumWantViewBy </summary>
  public class ForumWantViewBy : Item, INullRelationship<Forum>
  {
    protected ForumWantViewBy() { }
    public ForumWantViewBy(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ForumWantViewBy() { Innovator.Client.Item.AddNullItem<ForumWantViewBy>(new ForumWantViewBy { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>want_view_id</c> property of the item</summary>
    public IProperty_Item<Identity> WantViewId()
    {
      return this.Property("want_view_id");
    }
  }
}