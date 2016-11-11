using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Column Event </summary>
  public class ColumnEvent : Item, INullRelationship<GridColumn>, IRelationship<Method>
  {
    protected ColumnEvent() { }
    public ColumnEvent(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ColumnEvent() { Innovator.Client.Item.AddNullItem<ColumnEvent>(new ColumnEvent { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>grid_event</c> property of the item</summary>
    public IProperty_Text GridEvent()
    {
      return this.Property("grid_event");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}