using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Metric </summary>
  public class Metric : Item
  {
    protected Metric() { }
    public Metric(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>color</c> property of the item</summary>
    public IProperty_Text Color()
    {
      return this.Property("color");
    }
    /// <summary>Retrieve the <c>frequency</c> property of the item</summary>
    public IProperty_Text Frequency()
    {
      return this.Property("frequency");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>link</c> property of the item</summary>
    public IProperty_Text Link()
    {
      return this.Property("link");
    }
    /// <summary>Retrieve the <c>method</c> property of the item</summary>
    public IProperty_Item Method()
    {
      return this.Property("method");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>stylesheet</c> property of the item</summary>
    public IProperty_Text Stylesheet()
    {
      return this.Property("stylesheet");
    }
  }
}