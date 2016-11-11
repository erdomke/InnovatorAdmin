using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_PropertyType </summary>
  public class cmf_PropertyType : Item, INullRelationship<cmf_ElementType>
  {
    protected cmf_PropertyType() { }
    public cmf_PropertyType(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_PropertyType() { Innovator.Client.Item.AddNullItem<cmf_PropertyType>(new cmf_PropertyType { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>data_length</c> property of the item</summary>
    public IProperty_Number DataLength()
    {
      return this.Property("data_length");
    }
    /// <summary>Retrieve the <c>data_type</c> property of the item</summary>
    public IProperty_Text DataType()
    {
      return this.Property("data_type");
    }
    /// <summary>Retrieve the <c>default_permission</c> property of the item</summary>
    public IProperty_Item<Permission> DefaultPermission()
    {
      return this.Property("default_permission");
    }
    /// <summary>Retrieve the <c>generated_type</c> property of the item</summary>
    public IProperty_Item<ItemType> GeneratedType()
    {
      return this.Property("generated_type");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}