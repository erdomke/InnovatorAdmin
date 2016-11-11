using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ElementType </summary>
  public class cmf_ElementType : Item, INullRelationship<cmf_ContentType>
  {
    protected cmf_ElementType() { }
    public cmf_ElementType(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ElementType() { Innovator.Client.Item.AddNullItem<cmf_ElementType>(new cmf_ElementType { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
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
    /// <summary>Retrieve the <c>parent</c> property of the item</summary>
    public IProperty_Item<cmf_ElementType> ParentProp()
    {
      return this.Property("parent");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}