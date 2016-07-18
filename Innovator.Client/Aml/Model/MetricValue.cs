using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Metric Value </summary>
  public class MetricValue : Item
  {
    protected MetricValue() { }
    public MetricValue(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>calculate</c> property of the item</summary>
    public IProperty_Boolean Calculate()
    {
      return this.Property("calculate");
    }
    /// <summary>Retrieve the <c>color</c> property of the item</summary>
    public IProperty_Text Color()
    {
      return this.Property("color");
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
    /// <summary>Retrieve the <c>query</c> property of the item</summary>
    public IProperty_Text Query()
    {
      return this.Property("query");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>value</c> property of the item</summary>
    public IProperty_Number ValueProp()
    {
      return this.Property("value");
    }
  }
}