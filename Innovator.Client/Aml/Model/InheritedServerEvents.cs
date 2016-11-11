using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Inherited Server Events </summary>
  public class InheritedServerEvents : Item, INullRelationship<ItemType>
  {
    protected InheritedServerEvents() { }
    public InheritedServerEvents(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static InheritedServerEvents() { Innovator.Client.Item.AddNullItem<InheritedServerEvents>(new InheritedServerEvents { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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