using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ExportToExcelViewConfig </summary>
  public class cmf_ExportToExcelViewConfig : Item, INullRelationship<cmf_ContentTypeExportToExcel>
  {
    protected cmf_ExportToExcelViewConfig() { }
    public cmf_ExportToExcelViewConfig(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ExportToExcelViewConfig() { Innovator.Client.Item.AddNullItem<cmf_ExportToExcelViewConfig>(new cmf_ExportToExcelViewConfig { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>include_header</c> property of the item</summary>
    public IProperty_Boolean IncludeHeader()
    {
      return this.Property("include_header");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>starting_row_number</c> property of the item</summary>
    public IProperty_Number StartingRowNumber()
    {
      return this.Property("starting_row_number");
    }
    /// <summary>Retrieve the <c>tabular_view</c> property of the item</summary>
    public IProperty_Item<cmf_TabularView> TabularView()
    {
      return this.Property("tabular_view");
    }
    /// <summary>Retrieve the <c>use_style_settings</c> property of the item</summary>
    public IProperty_Boolean UseStyleSettings()
    {
      return this.Property("use_style_settings");
    }
    /// <summary>Retrieve the <c>worksheet_name</c> property of the item</summary>
    public IProperty_Text WorksheetName()
    {
      return this.Property("worksheet_name");
    }
  }
}