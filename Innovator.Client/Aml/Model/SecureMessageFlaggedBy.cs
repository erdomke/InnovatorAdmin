using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SecureMessageFlaggedBy </summary>
  public class SecureMessageFlaggedBy : Item
  {
    protected SecureMessageFlaggedBy() { }
    public SecureMessageFlaggedBy(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SecureMessageFlaggedBy() { Innovator.Client.Item.AddNullItem<SecureMessageFlaggedBy>(new SecureMessageFlaggedBy { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>flagged_by_id</c> property of the item</summary>
    public IProperty_Item<User> FlaggedById()
    {
      return this.Property("flagged_by_id");
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