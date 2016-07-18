using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_TabularViewColumn </summary>
  public class cmf_TabularViewColumn : Item
  {
    protected cmf_TabularViewColumn() { }
    public cmf_TabularViewColumn(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>col_order</c> property of the item</summary>
    public IProperty_Number ColOrder()
    {
      return this.Property("col_order");
    }
    /// <summary>Retrieve the <c>content_style</c> property of the item</summary>
    public IProperty_Item ContentStyle()
    {
      return this.Property("content_style");
    }
    /// <summary>Retrieve the <c>date_pattern</c> property of the item</summary>
    public IProperty_Text DatePattern()
    {
      return this.Property("date_pattern");
    }
    /// <summary>Retrieve the <c>editor_data_source_list</c> property of the item</summary>
    public IProperty_Item EditorDataSourceList()
    {
      return this.Property("editor_data_source_list");
    }
    /// <summary>Retrieve the <c>editor_data_source_method</c> property of the item</summary>
    public IProperty_Item EditorDataSourceMethod()
    {
      return this.Property("editor_data_source_method");
    }
    /// <summary>Retrieve the <c>editor_header_1_for_list_label</c> property of the item</summary>
    public IProperty_Text EditorHeader1ForListLabel()
    {
      return this.Property("editor_header_1_for_list_label");
    }
    /// <summary>Retrieve the <c>editor_header_1_width</c> property of the item</summary>
    public IProperty_Number EditorHeader1Width()
    {
      return this.Property("editor_header_1_width");
    }
    /// <summary>Retrieve the <c>editor_header_2_for_list_value</c> property of the item</summary>
    public IProperty_Text EditorHeader2ForListValue()
    {
      return this.Property("editor_header_2_for_list_value");
    }
    /// <summary>Retrieve the <c>editor_header_2_width</c> property of the item</summary>
    public IProperty_Number EditorHeader2Width()
    {
      return this.Property("editor_header_2_width");
    }
    /// <summary>Retrieve the <c>editor_use_both</c> property of the item</summary>
    public IProperty_Boolean EditorUseBoth()
    {
      return this.Property("editor_use_both");
    }
    /// <summary>Retrieve the <c>header</c> property of the item</summary>
    public IProperty_Text Header()
    {
      return this.Property("header");
    }
    /// <summary>Retrieve the <c>header_style</c> property of the item</summary>
    public IProperty_Item HeaderStyle()
    {
      return this.Property("header_style");
    }
    /// <summary>Retrieve the <c>initial_width</c> property of the item</summary>
    public IProperty_Number InitialWidth()
    {
      return this.Property("initial_width");
    }
    /// <summary>Retrieve the <c>property</c> property of the item</summary>
    public IProperty_Item PropertyProp()
    {
      return this.Property("property");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}