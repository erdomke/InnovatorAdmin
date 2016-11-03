using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Workflow Process Path Post </summary>
  public class WorkflowProcessPathPost : Item, INullRelationship<WorkflowProcessPath>, IRelationship<Method>
  {
    protected WorkflowProcessPathPost() { }
    public WorkflowProcessPathPost(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static WorkflowProcessPathPost() { Innovator.Client.Item.AddNullItem<WorkflowProcessPathPost>(new WorkflowProcessPathPost { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>sequence</c> property of the item</summary>
    public IProperty_Number Sequence()
    {
      return this.Property("sequence");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}