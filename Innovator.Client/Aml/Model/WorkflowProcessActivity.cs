using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Workflow Process Activity </summary>
  public class WorkflowProcessActivity : Item, INullRelationship<WorkflowProcess>, IRelationship<Activity>
  {
    protected WorkflowProcessActivity() { }
    public WorkflowProcessActivity(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static WorkflowProcessActivity() { Innovator.Client.Item.AddNullItem<WorkflowProcessActivity>(new WorkflowProcessActivity { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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