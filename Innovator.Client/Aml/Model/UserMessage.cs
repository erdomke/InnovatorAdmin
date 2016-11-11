using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type UserMessage </summary>
  public class UserMessage : Item
  {
    protected UserMessage() { }
    public UserMessage(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static UserMessage() { Innovator.Client.Item.AddNullItem<UserMessage>(new UserMessage { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>custom</c> property of the item</summary>
    public IProperty_Boolean Custom()
    {
      return this.Property("custom");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>text</c> property of the item</summary>
    public IProperty_Text Text()
    {
      return this.Property("text");
    }
  }
}