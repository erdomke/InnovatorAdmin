using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type History Template </summary>
  public class HistoryTemplate : Item
  {
    protected HistoryTemplate() { }
    public HistoryTemplate(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static HistoryTemplate() { Innovator.Client.Item.AddNullItem<HistoryTemplate>(new HistoryTemplate { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>property_history</c> property of the item</summary>
    public IProperty_Boolean PropertyHistory()
    {
      return this.Property("property_history");
    }
  }
}