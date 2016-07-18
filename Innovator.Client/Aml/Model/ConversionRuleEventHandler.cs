using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ConversionRuleEventHandler </summary>
  public class ConversionRuleEventHandler : Item
  {
    protected ConversionRuleEventHandler() { }
    public ConversionRuleEventHandler(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>event_type</c> property of the item</summary>
    public IProperty_Text EventType()
    {
      return this.Property("event_type");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}