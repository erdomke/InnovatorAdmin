using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type LockedItems </summary>
  public class LockedItems : Item, IRelationship<User>
  {
    protected LockedItems() { }
    public LockedItems(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static LockedItems() { Innovator.Client.Item.AddNullItem<LockedItems>(new LockedItems { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
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
    /// <summary>Retrieve the <c>source_type</c> property of the item</summary>
    public IProperty_Item<ItemType> SourceType()
    {
      return this.Property("source_type");
    }
  }
}