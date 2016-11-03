using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_PropertyAllowedPermission </summary>
  public class cmf_PropertyAllowedPermission : Item, INullRelationship<cmf_PropertyType>, IRelationship<Permission>
  {
    protected cmf_PropertyAllowedPermission() { }
    public cmf_PropertyAllowedPermission(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_PropertyAllowedPermission() { Innovator.Client.Item.AddNullItem<cmf_PropertyAllowedPermission>(new cmf_PropertyAllowedPermission { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

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