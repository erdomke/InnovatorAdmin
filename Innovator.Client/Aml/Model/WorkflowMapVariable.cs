using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Workflow Map Variable </summary>
  public class WorkflowMapVariable : Item, INullRelationship<WorkflowMap>
  {
    protected WorkflowMapVariable() { }
    public WorkflowMapVariable(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static WorkflowMapVariable() { Innovator.Client.Item.AddNullItem<WorkflowMapVariable>(new WorkflowMapVariable { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>datatype</c> property of the item</summary>
    public IProperty_Text Datatype()
    {
      return this.Property("datatype");
    }
    /// <summary>Retrieve the <c>default_value</c> property of the item</summary>
    public IProperty_Text DefaultValue()
    {
      return this.Property("default_value");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>source</c> property of the item</summary>
    public IProperty_Item<List> Source()
    {
      return this.Property("source");
    }
  }
}