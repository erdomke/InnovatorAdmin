using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Server Event </summary>
  public class ServerEvent : Item, INullRelationship<ItemType>, IRelationship<Method>
  {
    protected ServerEvent() { }
    public ServerEvent(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ServerEvent() { Innovator.Client.Item.AddNullItem<ServerEvent>(new ServerEvent { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>event_version</c> property of the item</summary>
    public IProperty_Text EventVersion()
    {
      return this.Property("event_version");
    }
    /// <summary>Retrieve the <c>is_required</c> property of the item</summary>
    public IProperty_Boolean IsRequired()
    {
      return this.Property("is_required");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>server_event</c> property of the item</summary>
    public IProperty_Text ServerEventProp()
    {
      return this.Property("server_event");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}