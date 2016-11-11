using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type PackageGroup </summary>
  public class PackageGroup : Item, INullRelationship<PackageDefinition>
  {
    protected PackageGroup() { }
    public PackageGroup(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static PackageGroup() { Innovator.Client.Item.AddNullItem<PackageGroup>(new PackageGroup { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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