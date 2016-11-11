using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type PackageElement </summary>
  public class PackageElement : Item, INullRelationship<PackageGroup>
  {
    protected PackageElement() { }
    public PackageElement(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static PackageElement() { Innovator.Client.Item.AddNullItem<PackageElement>(new PackageElement { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>element_id</c> property of the item</summary>
    public IProperty_Text ElementId()
    {
      return this.Property("element_id");
    }
    /// <summary>Retrieve the <c>element_type</c> property of the item</summary>
    public IProperty_Text ElementType()
    {
      return this.Property("element_type");
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