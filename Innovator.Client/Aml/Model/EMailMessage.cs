using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type EMail Message </summary>
  public class EMailMessage : Item
  {
    protected EMailMessage() { }
    public EMailMessage(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>body_html</c> property of the item</summary>
    public IProperty_Text BodyHtml()
    {
      return this.Property("body_html");
    }
    /// <summary>Retrieve the <c>body_plain</c> property of the item</summary>
    public IProperty_Text BodyPlain()
    {
      return this.Property("body_plain");
    }
    /// <summary>Retrieve the <c>from_user</c> property of the item</summary>
    public IProperty_Item FromUser()
    {
      return this.Property("from_user");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>query_string</c> property of the item</summary>
    public IProperty_Text QueryString()
    {
      return this.Property("query_string");
    }
    /// <summary>Retrieve the <c>subject</c> property of the item</summary>
    public IProperty_Text Subject()
    {
      return this.Property("subject");
    }
  }
}