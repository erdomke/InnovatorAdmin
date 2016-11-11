using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Discussion </summary>
  public class Discussion : Item, IRelationship<User>
  {
    protected Discussion() { }
    public Discussion(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Discussion() { Innovator.Client.Item.AddNullItem<Discussion>(new Discussion { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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
    /// <summary>Retrieve the <c>source_type</c> property of the item</summary>
    public IProperty_Item<ItemType> SourceType()
    {
      return this.Property("source_type");
    }
  }
}