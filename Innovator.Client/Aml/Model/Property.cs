using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Property </summary>
  public class Property : Item, INullRelationship<ItemType>
  {
    protected Property() { }
    public Property(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Property() { Innovator.Client.Item.AddNullItem<Property>(new Property { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>category_by</c> property of the item</summary>
    public IProperty_Number CategoryBy()
    {
      return this.Property("category_by");
    }
    /// <summary>Retrieve the <c>class_path</c> property of the item</summary>
    public IProperty_Text ClassPath()
    {
      return this.Property("class_path");
    }
    /// <summary>Retrieve the <c>column_alignment</c> property of the item</summary>
    public IProperty_Text ColumnAlignment()
    {
      return this.Property("column_alignment");
    }
    /// <summary>Retrieve the <c>column_width</c> property of the item</summary>
    public IProperty_Number ColumnWidth()
    {
      return this.Property("column_width");
    }
    /// <summary>Retrieve the <c>data_source</c> property of the item</summary>
    public IProperty_Item<ItemType> DataSource()
    {
      return this.Property("data_source");
    }
    /// <summary>Retrieve the <c>data_type</c> property of the item</summary>
    public IProperty_Text DataType()
    {
      return this.Property("data_type");
    }
    /// <summary>Retrieve the <c>default_search</c> property of the item</summary>
    public IProperty_Text DefaultSearch()
    {
      return this.Property("default_search");
    }
    /// <summary>Retrieve the <c>default_value</c> property of the item</summary>
    public IProperty_Text DefaultValue()
    {
      return this.Property("default_value");
    }
    /// <summary>Retrieve the <c>foreign_property</c> property of the item</summary>
    public IProperty_Item<Property> ForeignProperty()
    {
      return this.Property("foreign_property");
    }
    /// <summary>Retrieve the <c>help_text</c> property of the item</summary>
    public IProperty_Text HelpText()
    {
      return this.Property("help_text");
    }
    /// <summary>Retrieve the <c>help_tooltip</c> property of the item</summary>
    public IProperty_Text HelpTooltip()
    {
      return this.Property("help_tooltip");
    }
    /// <summary>Retrieve the <c>is_hidden</c> property of the item</summary>
    public IProperty_Boolean IsHidden()
    {
      return this.Property("is_hidden");
    }
    /// <summary>Retrieve the <c>is_hidden2</c> property of the item</summary>
    public IProperty_Boolean IsHidden2()
    {
      return this.Property("is_hidden2");
    }
    /// <summary>Retrieve the <c>is_indexed</c> property of the item</summary>
    public IProperty_Boolean IsIndexed()
    {
      return this.Property("is_indexed");
    }
    /// <summary>Retrieve the <c>is_keyed</c> property of the item</summary>
    public IProperty_Boolean IsKeyed()
    {
      return this.Property("is_keyed");
    }
    /// <summary>Retrieve the <c>is_multi_valued</c> property of the item</summary>
    public IProperty_Boolean IsMultiValued()
    {
      return this.Property("is_multi_valued");
    }
    /// <summary>Retrieve the <c>is_required</c> property of the item</summary>
    public IProperty_Boolean IsRequired()
    {
      return this.Property("is_required");
    }
    /// <summary>Retrieve the <c>item_behavior</c> property of the item</summary>
    public IProperty_Text ItemBehavior()
    {
      return this.Property("item_behavior");
    }
    /// <summary>Retrieve the <c>keyed_name_order</c> property of the item</summary>
    public IProperty_Number KeyedNameOrder()
    {
      return this.Property("keyed_name_order");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>max_range</c> property of the item</summary>
    public IProperty_Number MaxRange()
    {
      return this.Property("max_range");
    }
    /// <summary>Retrieve the <c>min_range</c> property of the item</summary>
    public IProperty_Number MinRange()
    {
      return this.Property("min_range");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>order_by</c> property of the item</summary>
    public IProperty_Number OrderBy()
    {
      return this.Property("order_by");
    }
    /// <summary>Retrieve the <c>pattern</c> property of the item</summary>
    public IProperty_Text Pattern()
    {
      return this.Property("pattern");
    }
    /// <summary>Retrieve the <c>prec</c> property of the item</summary>
    public IProperty_Number Prec()
    {
      return this.Property("prec");
    }
    /// <summary>Retrieve the <c>range_inclusive</c> property of the item</summary>
    public IProperty_Boolean RangeInclusive()
    {
      return this.Property("range_inclusive");
    }
    /// <summary>Retrieve the <c>readonly</c> property of the item</summary>
    public IProperty_Boolean Readonly()
    {
      return this.Property("readonly");
    }
    /// <summary>Retrieve the <c>scale</c> property of the item</summary>
    public IProperty_Number Scale()
    {
      return this.Property("scale");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>stored_length</c> property of the item</summary>
    public IProperty_Number StoredLength()
    {
      return this.Property("stored_length");
    }
    /// <summary>Retrieve the <c>track_history</c> property of the item</summary>
    public IProperty_Boolean TrackHistory()
    {
      return this.Property("track_history");
    }
  }
}