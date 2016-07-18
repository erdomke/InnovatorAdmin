using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ForumMustViewBy </summary>
  public class ForumMustViewBy : Item
  {
    protected ForumMustViewBy() { }
    public ForumMustViewBy(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>must_view_id</c> property of the item</summary>
    public IProperty_Item MustViewId()
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