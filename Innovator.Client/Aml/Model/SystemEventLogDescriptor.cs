using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SystemEventLogDescriptor </summary>
  public class SystemEventLogDescriptor : Item
  {
    protected SystemEventLogDescriptor() { }
    public SystemEventLogDescriptor(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>log_level</c> property of the item</summary>
    public IProperty_Number LogLevel()
    {
      return this.Property("log_level");
    }
    /// <summary>Retrieve the <c>log_message</c> property of the item</summary>
    public IProperty_Text LogMessage()
    {
      return this.Property("log_message");
    }
    /// <summary>Retrieve the <c>system_event</c> property of the item</summary>
    public IProperty_Item SystemEvent()
    {
      return this.Property("system_event");
    }
  }
}