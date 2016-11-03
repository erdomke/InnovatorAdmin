using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type State Notification </summary>
  public class StateNotification : Item, INullRelationship<LifeCycleState>, IRelationship<Identity>
  {
    protected StateNotification() { }
    public StateNotification(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static StateNotification() { Innovator.Client.Item.AddNullItem<StateNotification>(new StateNotification { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>body</c> property of the item</summary>
    public IProperty_Text Body()
    {
      return this.Property("body");
    }
    /// <summary>Retrieve the <c>from_user</c> property of the item</summary>
    public IProperty_Item<User> FromUser()
    {
      return this.Property("from_user");
    }
    /// <summary>Retrieve the <c>from_user_str</c> property of the item</summary>
    public IProperty_Text FromUserStr()
    {
      return this.Property("from_user_str");
    }
    /// <summary>Retrieve the <c>html_body</c> property of the item</summary>
    public IProperty_Text HtmlBody()
    {
      return this.Property("html_body");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>subject</c> property of the item</summary>
    public IProperty_Text Subject()
    {
      return this.Property("subject");
    }
  }
}