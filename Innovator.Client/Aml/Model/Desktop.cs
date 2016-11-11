using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Desktop </summary>
  public class Desktop : Item, IRelationship<User>
  {
    protected Desktop() { }
    public Desktop(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Desktop() { Innovator.Client.Item.AddNullItem<Desktop>(new Desktop { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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
    /// <summary>Retrieve the <c>x_pos</c> property of the item</summary>
    public IProperty_Number XPos()
    {
      return this.Property("x_pos");
    }
    /// <summary>Retrieve the <c>y_pos</c> property of the item</summary>
    public IProperty_Number YPos()
    {
      return this.Property("y_pos");
    }
  }
}