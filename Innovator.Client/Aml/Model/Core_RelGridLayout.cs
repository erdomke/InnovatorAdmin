using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Core_RelGridLayout </summary>
  public class Core_RelGridLayout : Item
  {
    protected Core_RelGridLayout() { }
    public Core_RelGridLayout(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
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
    /// <summary>Retrieve the <c>page_size</c> property of the item</summary>
    public IProperty_Number PageSize()
    {
      return this.Property("page_size");
    }
    /// <summary>Retrieve the <c>redline_view</c> property of the item</summary>
    public IProperty_Boolean RedlineView()
    {
      return this.Property("redline_view");
    }
    /// <summary>Retrieve the <c>rel_type_id</c> property of the item</summary>
    public IProperty_Text RelTypeId()
    {
      return this.Property("rel_type_id");
    }
    /// <summary>Retrieve the <c>search_vis</c> property of the item</summary>
    public IProperty_Text SearchVis()
    {
      return this.Property("search_vis");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}