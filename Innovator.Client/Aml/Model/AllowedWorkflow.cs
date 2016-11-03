using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Allowed Workflow </summary>
  public class AllowedWorkflow : Item, INullRelationship<ItemType>, IRelationship<WorkflowMap>
  {
    protected AllowedWorkflow() { }
    public AllowedWorkflow(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static AllowedWorkflow() { Innovator.Client.Item.AddNullItem<AllowedWorkflow>(new AllowedWorkflow { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>is_default</c> property of the item</summary>
    public IProperty_Boolean IsDefault()
    {
      return this.Property("is_default");
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