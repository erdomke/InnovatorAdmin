using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type DiscussionDefinition </summary>
  public class DiscussionDefinition : Item
  {
    protected DiscussionDefinition() { }
    public DiscussionDefinition(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static DiscussionDefinition() { Innovator.Client.Item.AddNullItem<DiscussionDefinition>(new DiscussionDefinition { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>file_selection_depth</c> property of the item</summary>
    public IProperty_Number FileSelectionDepth()
    {
      return this.Property("file_selection_depth");
    }
    /// <summary>Retrieve the <c>item_config_id</c> property of the item</summary>
    public IProperty_Text ItemConfigId()
    {
      return this.Property("item_config_id");
    }
    /// <summary>Retrieve the <c>item_selection_depth</c> property of the item</summary>
    public IProperty_Number ItemSelectionDepth()
    {
      return this.Property("item_selection_depth");
    }
    /// <summary>Retrieve the <c>item_type_name</c> property of the item</summary>
    public IProperty_Text ItemTypeName()
    {
      return this.Property("item_type_name");
    }
    /// <summary>Retrieve the <c>template_id</c> property of the item</summary>
    public IProperty_Text TemplateId()
    {
      return this.Property("template_id");
    }
  }
}