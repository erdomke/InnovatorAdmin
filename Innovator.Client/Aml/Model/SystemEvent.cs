using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SystemEvent </summary>
  public class SystemEvent : Item
  {
    protected SystemEvent() { }
    public SystemEvent(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SystemEvent() { Innovator.Client.Item.AddNullItem<SystemEvent>(new SystemEvent { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>event_type</c> property of the item</summary>
    public IProperty_Text EventType()
    {
      return this.Property("event_type");
    }
  }
}