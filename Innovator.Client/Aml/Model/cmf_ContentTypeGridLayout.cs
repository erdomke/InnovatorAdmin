using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ContentTypeGridLayout </summary>
  public class cmf_ContentTypeGridLayout : Item, INullRelationship<Preference>
  {
    protected cmf_ContentTypeGridLayout() { }
    public cmf_ContentTypeGridLayout(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ContentTypeGridLayout() { Innovator.Client.Item.AddNullItem<cmf_ContentTypeGridLayout>(new cmf_ContentTypeGridLayout { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>collapsed_column_groups</c> property of the item</summary>
    public IProperty_Text CollapsedColumnGroups()
    {
      return this.Property("collapsed_column_groups");
    }
    /// <summary>Retrieve the <c>column_names</c> property of the item</summary>
    public IProperty_Text ColumnNames()
    {
      return this.Property("column_names");
    }
    /// <summary>Retrieve the <c>column_width</c> property of the item</summary>
    public IProperty_Text ColumnWidth()
    {
      return this.Property("column_width");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>tabular_view_id</c> property of the item</summary>
    public IProperty_Text TabularViewId()
    {
      return this.Property("tabular_view_id");
    }
  }
}