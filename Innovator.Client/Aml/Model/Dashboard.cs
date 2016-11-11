using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Dashboard </summary>
  public class Dashboard : Item
  {
    protected Dashboard() { }
    public Dashboard(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Dashboard() { Innovator.Client.Item.AddNullItem<Dashboard>(new Dashboard { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>background_style</c> property of the item</summary>
    public IProperty_Text BackgroundStyle()
    {
      return this.Property("background_style");
    }
    /// <summary>Retrieve the <c>height</c> property of the item</summary>
    public IProperty_Number Height()
    {
      return this.Property("height");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>width</c> property of the item</summary>
    public IProperty_Number Width()
    {
      return this.Property("width");
    }
  }
}