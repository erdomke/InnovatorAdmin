using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Client Event </summary>
  public class ClientEvent : Item, INullRelationship<ItemType>, IRelationship<Method>
  {
    protected ClientEvent() { }
    public ClientEvent(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ClientEvent() { Innovator.Client.Item.AddNullItem<ClientEvent>(new ClientEvent { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>client_event</c> property of the item</summary>
    public IProperty_Text ClientEventProp()
    {
      return this.Property("client_event");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}