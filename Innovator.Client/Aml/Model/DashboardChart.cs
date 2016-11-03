using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Dashboard Chart </summary>
  public class DashboardChart : Item, INullRelationship<Dashboard>, IRelationship<Chart>
  {
    protected DashboardChart() { }
    public DashboardChart(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static DashboardChart() { Innovator.Client.Item.AddNullItem<DashboardChart>(new DashboardChart { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>x</c> property of the item</summary>
    public IProperty_Number X()
    {
      return this.Property("x");
    }
    /// <summary>Retrieve the <c>y</c> property of the item</summary>
    public IProperty_Number Y()
    {
      return this.Property("y");
    }
  }
}