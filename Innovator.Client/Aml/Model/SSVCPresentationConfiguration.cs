using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SSVCPresentationConfiguration </summary>
  public class SSVCPresentationConfiguration : Item
  {
    protected SSVCPresentationConfiguration() { }
    public SSVCPresentationConfiguration(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SSVCPresentationConfiguration() { Innovator.Client.Item.AddNullItem<SSVCPresentationConfiguration>(new SSVCPresentationConfiguration { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>discussion_panel_behavior</c> property of the item</summary>
    public IProperty_Text DiscussionPanelBehavior()
    {
      return this.Property("discussion_panel_behavior");
    }
    /// <summary>Retrieve the <c>form_tooltip_template</c> property of the item</summary>
    public IProperty_Text FormTooltipTemplate()
    {
      return this.Property("form_tooltip_template");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>sm_template</c> property of the item</summary>
    public IProperty_Item<SecureMessageViewTemplate> SmTemplate()
    {
      return this.Property("sm_template");
    }
  }
}