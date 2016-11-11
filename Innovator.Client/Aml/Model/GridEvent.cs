using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Grid Event </summary>
  public class GridEvent : Item, INullRelationship<Property>, IRelationship<Method>
  {
    protected GridEvent() { }
    public GridEvent(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static GridEvent() { Innovator.Client.Item.AddNullItem<GridEvent>(new GridEvent { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>grid_event</c> property of the item</summary>
    public IProperty_Text GridEventProp()
    {
      return this.Property("grid_event");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}