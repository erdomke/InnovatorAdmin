using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Core_ItemGridLayout </summary>
  public class Core_ItemGridLayout : Item, INullRelationship<Preference>
  {
    protected Core_ItemGridLayout() { }
    public Core_ItemGridLayout(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Core_ItemGridLayout() { Innovator.Client.Item.AddNullItem<Core_ItemGridLayout>(new Core_ItemGridLayout { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>col_order</c> property of the item</summary>
    public IProperty_Text ColOrder()
    {
      return this.Property("col_order");
    }
    /// <summary>Retrieve the <c>col_widths</c> property of the item</summary>
    public IProperty_Text ColWidths()
    {
      return this.Property("col_widths");
    }
    /// <summary>Retrieve the <c>item_type_id</c> property of the item</summary>
    public IProperty_Text ItemTypeId()
    {
      return this.Property("item_type_id");
    }
    /// <summary>Retrieve the <c>max_records</c> property of the item</summary>
    public IProperty_Number MaxRecords()
    {
      return this.Property("max_records");
    }
    /// <summary>Retrieve the <c>page_size</c> property of the item</summary>
    public IProperty_Number PageSize()
    {
      return this.Property("page_size");
    }
    /// <summary>Retrieve the <c>query_type</c> property of the item</summary>
    public IProperty_Text QueryType()
    {
      return this.Property("query_type");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}