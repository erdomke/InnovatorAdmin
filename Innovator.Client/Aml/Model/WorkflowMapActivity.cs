using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Workflow Map Activity </summary>
  public class WorkflowMapActivity : Item, INullRelationship<WorkflowMap>, IRelationship<ActivityTemplate>
  {
    protected WorkflowMapActivity() { }
    public WorkflowMapActivity(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static WorkflowMapActivity() { Innovator.Client.Item.AddNullItem<WorkflowMapActivity>(new WorkflowMapActivity { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

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
  }
}