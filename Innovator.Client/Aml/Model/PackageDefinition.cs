using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type PackageDefinition </summary>
  public class PackageDefinition : Item
  {
    protected PackageDefinition() { }
    public PackageDefinition(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static PackageDefinition() { Innovator.Client.Item.AddNullItem<PackageDefinition>(new PackageDefinition { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
  }
}