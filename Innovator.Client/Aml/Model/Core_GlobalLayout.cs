using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Core_GlobalLayout </summary>
  public class Core_GlobalLayout : Item
  {
    protected Core_GlobalLayout() { }
    public Core_GlobalLayout(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>core_append_items</c> property of the item</summary>
    public IProperty_Text CoreAppendItems()
    {
      return this.Property("core_append_items");
    }
    /// <summary>Retrieve the <c>core_debug</c> property of the item</summary>
    public IProperty_Text CoreDebug()
    {
      return this.Property("core_debug");
    }
    /// <summary>Retrieve the <c>core_default_download_location</c> property of the item</summary>
    public IProperty_Number CoreDefaultDownloadLocation()
    {
      return this.Property("core_default_download_location");
    }
    /// <summary>Retrieve the <c>core_in_basket_history</c> property of the item</summary>
    public IProperty_Text CoreInBasketHistory()
    {
      return this.Property("core_in_basket_history");
    }
    /// <summary>Retrieve the <c>core_popupmessage_timeout</c> property of the item</summary>
    public IProperty_Number CorePopupmessageTimeout()
    {
      return this.Property("core_popupmessage_timeout");
    }
    /// <summary>Retrieve the <c>core_show_item_properties</c> property of the item</summary>
    public IProperty_Text CoreShowItemProperties()
    {
      return this.Property("core_show_item_properties");
    }
    /// <summary>Retrieve the <c>core_show_labels</c> property of the item</summary>
    public IProperty_Text CoreShowLabels()
    {
      return this.Property("core_show_labels");
    }
    /// <summary>Retrieve the <c>core_show_scan_button_for_file</c> property of the item</summary>
    public IProperty_Boolean CoreShowScanButtonForFile()
    {
      return this.Property("core_show_scan_button_for_file");
    }
    /// <summary>Retrieve the <c>core_structure_layout</c> property of the item</summary>
    public IProperty_Text CoreStructureLayout()
    {
      return this.Property("core_structure_layout");
    }
    /// <summary>Retrieve the <c>core_successmessage_type</c> property of the item</summary>
    public IProperty_Text CoreSuccessmessageType()
    {
      return this.Property("core_successmessage_type");
    }
    /// <summary>Retrieve the <c>core_tabs_state</c> property of the item</summary>
    public IProperty_Text CoreTabsState()
    {
      return this.Property("core_tabs_state");
    }
    /// <summary>Retrieve the <c>core_tear_off</c> property of the item</summary>
    public IProperty_Text CoreTearOff()
    {
      return this.Property("core_tear_off");
    }
    /// <summary>Retrieve the <c>core_use_wildcards</c> property of the item</summary>
    public IProperty_Text CoreUseWildcards()
    {
      return this.Property("core_use_wildcards");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}