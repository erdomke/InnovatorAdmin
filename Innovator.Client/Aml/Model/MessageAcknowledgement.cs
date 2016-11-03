using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Message Acknowledgement </summary>
  public class MessageAcknowledgement : Item, INullRelationship<Message>, IRelationship<User>
  {
    protected MessageAcknowledgement() { }
    public MessageAcknowledgement(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static MessageAcknowledgement() { Innovator.Client.Item.AddNullItem<MessageAcknowledgement>(new MessageAcknowledgement { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

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