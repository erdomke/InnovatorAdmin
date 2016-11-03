using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Workflow </summary>
  public class Workflow : Item, IRelationship<WorkflowProcess>
  {
    protected Workflow() { }
    public Workflow(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Workflow() { Innovator.Client.Item.AddNullItem<Workflow>(new Workflow { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

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
    /// <summary>Retrieve the <c>source_type</c> property of the item</summary>
    public IProperty_Item<ItemType> SourceType()
    {
      return this.Property("source_type");
    }
  }
}