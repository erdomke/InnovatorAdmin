using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type PackageDependsOn </summary>
  public class PackageDependsOn : Item, INullRelationship<PackageDefinition>
  {
    protected PackageDependsOn() { }
    public PackageDependsOn(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static PackageDependsOn() { Innovator.Client.Item.AddNullItem<PackageDependsOn>(new PackageDependsOn { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
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