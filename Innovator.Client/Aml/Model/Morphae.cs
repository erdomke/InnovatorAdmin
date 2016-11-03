using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Morphae </summary>
  public class Morphae : Item, INullRelationship<ItemType>, IRelationship<ItemType>
  {
    protected Morphae() { }
    public Morphae(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Morphae() { Innovator.Client.Item.AddNullItem<Morphae>(new Morphae { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>implementation_order</c> property of the item</summary>
    public IProperty_Number ImplementationOrder()
    {
      return this.Property("implementation_order");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}