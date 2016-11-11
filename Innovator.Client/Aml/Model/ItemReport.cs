using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Item Report </summary>
  public class ItemReport : Item, INullRelationship<ItemType>, IRelationship<Report>
  {
    protected ItemReport() { }
    public ItemReport(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ItemReport() { Innovator.Client.Item.AddNullItem<ItemReport>(new ItemReport { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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
  }
}