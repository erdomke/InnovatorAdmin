using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SPField </summary>
  public class SPField : Item, INullRelationship<SPDocumentLibraryDefinition>
  {
    protected SPField() { }
    public SPField(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SPField() { Innovator.Client.Item.AddNullItem<SPField>(new SPField { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>property_hidden</c> property of the item</summary>
    public IProperty_Boolean PropertyHidden()
    {
      return this.Property("property_hidden");
    }
    /// <summary>Retrieve the <c>property_hidden2</c> property of the item</summary>
    public IProperty_Boolean PropertyHidden2()
    {
      return this.Property("property_hidden2");
    }
    /// <summary>Retrieve the <c>property_keyed_order</c> property of the item</summary>
    public IProperty_Number PropertyKeyedOrder()
    {
      return this.Property("property_keyed_order");
    }
    /// <summary>Retrieve the <c>property_label</c> property of the item</summary>
    public IProperty_Text PropertyLabel()
    {
      return this.Property("property_label");
    }
    /// <summary>Retrieve the <c>property_length</c> property of the item</summary>
    public IProperty_Number PropertyLength()
    {
      return this.Property("property_length");
    }
    /// <summary>Retrieve the <c>property_name</c> property of the item</summary>
    public IProperty_Text PropertyName()
    {
      return this.Property("property_name");
    }
    /// <summary>Retrieve the <c>property_type</c> property of the item</summary>
    public IProperty_Text PropertyType()
    {
      return this.Property("property_type");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>sp_field_name</c> property of the item</summary>
    public IProperty_Text SpFieldName()
    {
      return this.Property("sp_field_name");
    }
    /// <summary>Retrieve the <c>sp_field_type</c> property of the item</summary>
    public IProperty_Text SpFieldType()
    {
      return this.Property("sp_field_type");
    }
    /// <summary>Retrieve the <c>sp_guid</c> property of the item</summary>
    public IProperty_Text SpGuid()
    {
      return this.Property("sp_guid");
    }
  }
}