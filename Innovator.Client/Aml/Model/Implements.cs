using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Implements </summary>
  public class Implements : Item, INullRelationship<ItemType>
  {
    protected Implements() { }
    public Implements(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Implements() { Innovator.Client.Item.AddNullItem<Implements>(new Implements { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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