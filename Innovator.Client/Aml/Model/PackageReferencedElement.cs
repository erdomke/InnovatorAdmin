using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type PackageReferencedElement </summary>
  public class PackageReferencedElement : Item, INullRelationship<PackageDependsOn>
  {
    protected PackageReferencedElement() { }
    public PackageReferencedElement(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static PackageReferencedElement() { Innovator.Client.Item.AddNullItem<PackageReferencedElement>(new PackageReferencedElement { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>element_description</c> property of the item</summary>
    public IProperty_Text ElementDescription()
    {
      return this.Property("element_description");
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
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}