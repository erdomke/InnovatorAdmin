using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ElementAllowedPermission </summary>
  public class cmf_ElementAllowedPermission : Item, INullRelationship<cmf_ElementType>, IRelationship<Permission>
  {
    protected cmf_ElementAllowedPermission() { }
    public cmf_ElementAllowedPermission(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ElementAllowedPermission() { Innovator.Client.Item.AddNullItem<cmf_ElementAllowedPermission>(new cmf_ElementAllowedPermission { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}