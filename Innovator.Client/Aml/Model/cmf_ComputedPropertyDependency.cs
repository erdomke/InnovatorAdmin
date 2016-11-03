using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ComputedPropertyDependency </summary>
  public class cmf_ComputedPropertyDependency : Item, INullRelationship<cmf_ComputedProperty>, IRelationship<cmf_PropertyType>
  {
    protected cmf_ComputedPropertyDependency() { }
    public cmf_ComputedPropertyDependency(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ComputedPropertyDependency() { Innovator.Client.Item.AddNullItem<cmf_ComputedPropertyDependency>(new cmf_ComputedPropertyDependency { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

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