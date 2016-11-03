using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Workflow Process </summary>
  public class WorkflowProcess : Item
  {
    protected WorkflowProcess() { }
    public WorkflowProcess(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static WorkflowProcess() { Innovator.Client.Item.AddNullItem<WorkflowProcess>(new WorkflowProcess { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>active_date</c> property of the item</summary>
    public IProperty_Date ActiveDate()
    {
      return this.Property("active_date");
    }
    /// <summary>Retrieve the <c>closed_date</c> property of the item</summary>
    public IProperty_Date ClosedDate()
    {
      return this.Property("closed_date");
    }
    /// <summary>Retrieve the <c>copied_from</c> property of the item</summary>
    public IProperty_Item<WorkflowMap> CopiedFrom()
    {
      return this.Property("copied_from");
    }
    /// <summary>Retrieve the <c>copied_from_string</c> property of the item</summary>
    public IProperty_Text CopiedFromString()
    {
      return this.Property("copied_from_string");
    }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
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
    /// <summary>Retrieve the <c>node_bg_color</c> property of the item</summary>
    public IProperty_Text NodeBgColor()
    {
      return this.Property("node_bg_color");
    }
    /// <summary>Retrieve the <c>node_label1_color</c> property of the item</summary>
    public IProperty_Text NodeLabel1Color()
    {
      return this.Property("node_label1_color");
    }
    /// <summary>Retrieve the <c>node_label1_font</c> property of the item</summary>
    public IProperty_Text NodeLabel1Font()
    {
      return this.Property("node_label1_font");
    }
    /// <summary>Retrieve the <c>node_label2_color</c> property of the item</summary>
    public IProperty_Text NodeLabel2Color()
    {
      return this.Property("node_label2_color");
    }
    /// <summary>Retrieve the <c>node_label2_font</c> property of the item</summary>
    public IProperty_Text NodeLabel2Font()
    {
      return this.Property("node_label2_font");
    }
    /// <summary>Retrieve the <c>node_name_color</c> property of the item</summary>
    public IProperty_Text NodeNameColor()
    {
      return this.Property("node_name_color");
    }
    /// <summary>Retrieve the <c>node_name_font</c> property of the item</summary>
    public IProperty_Text NodeNameFont()
    {
      return this.Property("node_name_font");
    }
    /// <summary>Retrieve the <c>node_size</c> property of the item</summary>
    public IProperty_Text NodeSize()
    {
      return this.Property("node_size");
    }
    /// <summary>Retrieve the <c>process_owner</c> property of the item</summary>
    public IProperty_Item<Identity> ProcessOwner()
    {
      return this.Property("process_owner");
    }
    /// <summary>Retrieve the <c>sub_of</c> property of the item</summary>
    public IProperty_Item<Activity> SubOf()
    {
      return this.Property("sub_of");
    }
    /// <summary>Retrieve the <c>transition_line_color</c> property of the item</summary>
    public IProperty_Text TransitionLineColor()
    {
      return this.Property("transition_line_color");
    }
    /// <summary>Retrieve the <c>transition_name_color</c> property of the item</summary>
    public IProperty_Text TransitionNameColor()
    {
      return this.Property("transition_name_color");
    }
    /// <summary>Retrieve the <c>transition_name_font</c> property of the item</summary>
    public IProperty_Text TransitionNameFont()
    {
      return this.Property("transition_name_font");
    }
  }
}