using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Chart Series </summary>
  public class ChartSeries : Item, INullRelationship<Chart>, IRelationship<Metric>
  {
    protected ChartSeries() { }
    public ChartSeries(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ChartSeries() { Innovator.Client.Item.AddNullItem<ChartSeries>(new ChartSeries { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>markers</c> property of the item</summary>
    public IProperty_Boolean Markers()
    {
      return this.Property("markers");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}