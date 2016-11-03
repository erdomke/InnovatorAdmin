using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Help </summary>
  public class Help : Item
  {
    protected Help() { }
    public Help(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Help() { Innovator.Client.Item.AddNullItem<Help>(new Help { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>content</c> property of the item</summary>
    public IProperty_Text Content()
    {
      return this.Property("content");
    }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
  }
}