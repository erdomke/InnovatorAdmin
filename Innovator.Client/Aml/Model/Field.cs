using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Field </summary>
  public class Field : Item, INullRelationship<Body>
  {
    protected Field() { }
    public Field(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Field() { Innovator.Client.Item.AddNullItem<Field>(new Field { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>bg_color</c> property of the item</summary>
    public IProperty_Text BgColor()
    {
      return this.Property("bg_color");
    }
    /// <summary>Retrieve the <c>border_width</c> property of the item</summary>
    public IProperty_Number BorderWidth()
    {
      return this.Property("border_width");
    }
    /// <summary>Retrieve the <c>clip_overflow</c> property of the item</summary>
    public IProperty_Text ClipOverflow()
    {
      return this.Property("clip_overflow");
    }
    /// <summary>Retrieve the <c>clip_rectangle</c> property of the item</summary>
    public IProperty_Text ClipRectangle()
    {
      return this.Property("clip_rectangle");
    }
    /// <summary>Retrieve the <c>container</c> property of the item</summary>
    public IProperty_Text Container()
    {
      return this.Property("container");
    }
    /// <summary>Retrieve the <c>default_value</c> property of the item</summary>
    public IProperty_Text DefaultValue()
    {
      return this.Property("default_value");
    }
    /// <summary>Retrieve the <c>display_length</c> property of the item</summary>
    public IProperty_Number DisplayLength()
    {
      return this.Property("display_length");
    }
    /// <summary>Retrieve the <c>display_length_unit</c> property of the item</summary>
    public IProperty_Text DisplayLengthUnit()
    {
      return this.Property("display_length_unit");
    }
    /// <summary>Retrieve the <c>field_type</c> property of the item</summary>
    public IProperty_Text FieldType()
    {
      return this.Property("field_type");
    }
    /// <summary>Retrieve the <c>font_color</c> property of the item</summary>
    public IProperty_Text FontColor()
    {
      return this.Property("font_color");
    }
    /// <summary>Retrieve the <c>font_family</c> property of the item</summary>
    public IProperty_Text FontFamily()
    {
      return this.Property("font_family");
    }
    /// <summary>Retrieve the <c>font_size</c> property of the item</summary>
    public IProperty_Text FontSize()
    {
      return this.Property("font_size");
    }
    /// <summary>Retrieve the <c>font_weight</c> property of the item</summary>
    public IProperty_Text FontWeight()
    {
      return this.Property("font_weight");
    }
    /// <summary>Retrieve the <c>height</c> property of the item</summary>
    public IProperty_Number Height()
    {
      return this.Property("height");
    }
    /// <summary>Retrieve the <c>html_code</c> property of the item</summary>
    public IProperty_Text HtmlCode()
    {
      return this.Property("html_code");
    }
    /// <summary>Retrieve the <c>is_disabled</c> property of the item</summary>
    public IProperty_Boolean IsDisabled()
    {
      return this.Property("is_disabled");
    }
    /// <summary>Retrieve the <c>is_visible</c> property of the item</summary>
    public IProperty_Boolean IsVisible()
    {
      return this.Property("is_visible");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>label_position</c> property of the item</summary>
    public IProperty_Text LabelPosition()
    {
      return this.Property("label_position");
    }
    /// <summary>Retrieve the <c>legend</c> property of the item</summary>
    public IProperty_Text Legend()
    {
      return this.Property("legend");
    }
    /// <summary>Retrieve the <c>list_no_blank</c> property of the item</summary>
    public IProperty_Boolean ListNoBlank()
    {
      return this.Property("list_no_blank");
    }
    /// <summary>Retrieve the <c>listbox_size</c> property of the item</summary>
    public IProperty_Number ListboxSize()
    {
      return this.Property("listbox_size");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>orientation</c> property of the item</summary>
    public IProperty_Text Orientation()
    {
      return this.Property("orientation");
    }
    /// <summary>Retrieve the <c>positioning</c> property of the item</summary>
    public IProperty_Text Positioning()
    {
      return this.Property("positioning");
    }
    /// <summary>Retrieve the <c>propertytype_id</c> property of the item</summary>
    public IProperty_Item<Property> PropertytypeId()
    {
      return this.Property("propertytype_id");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>tab_index</c> property of the item</summary>
    public IProperty_Number TabIndex()
    {
      return this.Property("tab_index");
    }
    /// <summary>Retrieve the <c>tab_stop</c> property of the item</summary>
    public IProperty_Boolean TabStop()
    {
      return this.Property("tab_stop");
    }
    /// <summary>Retrieve the <c>text_align</c> property of the item</summary>
    public IProperty_Text TextAlign()
    {
      return this.Property("text_align");
    }
    /// <summary>Retrieve the <c>textarea_cols</c> property of the item</summary>
    public IProperty_Number TextareaCols()
    {
      return this.Property("textarea_cols");
    }
    /// <summary>Retrieve the <c>textarea_rows</c> property of the item</summary>
    public IProperty_Number TextareaRows()
    {
      return this.Property("textarea_rows");
    }
    /// <summary>Retrieve the <c>width</c> property of the item</summary>
    public IProperty_Number Width()
    {
      return this.Property("width");
    }
    /// <summary>Retrieve the <c>x</c> property of the item</summary>
    public IProperty_Number X()
    {
      return this.Property("x");
    }
    /// <summary>Retrieve the <c>y</c> property of the item</summary>
    public IProperty_Number Y()
    {
      return this.Property("y");
    }
    /// <summary>Retrieve the <c>z_index</c> property of the item</summary>
    public IProperty_Number ZIndex()
    {
      return this.Property("z_index");
    }
  }
}