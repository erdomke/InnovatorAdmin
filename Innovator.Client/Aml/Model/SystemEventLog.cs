using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SystemEventLog </summary>
  public class SystemEventLog : Item
  {
    protected SystemEventLog() { }
    public SystemEventLog(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>comment_text</c> property of the item</summary>
    public IProperty_Text CommentText()
    {
      return this.Property("comment_text");
    }
    /// <summary>Retrieve the <c>event_type</c> property of the item</summary>
    public IProperty_Text EventType()
    {
      return this.Property("event_type");
    }
    /// <summary>Retrieve the <c>ip_address</c> property of the item</summary>
    public IProperty_Text IpAddress()
    {
      return this.Property("ip_address");
    }
    /// <summary>Retrieve the <c>login_name</c> property of the item</summary>
    public IProperty_Text LoginName()
    {
      return this.Property("login_name");
    }
    /// <summary>Retrieve the <c>method_name</c> property of the item</summary>
    public IProperty_Text MethodName()
    {
      return this.Property("method_name");
    }
    /// <summary>Retrieve the <c>session_id</c> property of the item</summary>
    public IProperty_Text SessionId()
    {
      return this.Property("session_id");
    }
  }
}