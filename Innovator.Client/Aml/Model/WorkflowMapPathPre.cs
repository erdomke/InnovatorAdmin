using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Workflow Map Path Pre </summary>
  public class WorkflowMapPathPre : Item, INullRelationship<WorkflowMapPath>, IRelationship<Method>
  {
    protected WorkflowMapPathPre() { }
    public WorkflowMapPathPre(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static WorkflowMapPathPre() { Innovator.Client.Item.AddNullItem<WorkflowMapPathPre>(new WorkflowMapPathPre { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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