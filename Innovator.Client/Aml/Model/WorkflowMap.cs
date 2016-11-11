using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Workflow Map </summary>
  public class WorkflowMap : Item
  {
    protected WorkflowMap() { }
    public WorkflowMap(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static WorkflowMap() { Innovator.Client.Item.AddNullItem<WorkflowMap>(new WorkflowMap { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>comments</c> property of the item</summary>
    public IProperty_Text Comments()
    {
      return this.Property("comments");
    }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>effective_date</c> property of the item</summary>
    public IProperty_Date EffectiveDate()
    {
      return this.Property("effective_date");
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
    /// <summary>Retrieve the <c>release_date</c> property of the item</summary>
    public IProperty_Date ReleaseDate()
    {
      return this.Property("release_date");
    }
    /// <summary>Retrieve the <c>superseded_date</c> property of the item</summary>
    public IProperty_Date SupersededDate()
    {
      return this.Property("superseded_date");
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