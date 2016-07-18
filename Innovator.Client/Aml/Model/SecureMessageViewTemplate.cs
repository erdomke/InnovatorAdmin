using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SecureMessageViewTemplate </summary>
  public class SecureMessageViewTemplate : Item
  {
    protected SecureMessageViewTemplate() { }
    public SecureMessageViewTemplate(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>style</c> property of the item</summary>
    public IProperty_Text Style()
    {
      return this.Property("style");
    }
    /// <summary>Retrieve the <c>template</c> property of the item</summary>
    public IProperty_Text Template()
    {
      return this.Property("template");
    }
    /// <summary>Retrieve the <c>thumbnail_tooltip_template</c> property of the item</summary>
    public IProperty_Text ThumbnailTooltipTemplate()
    {
      return this.Property("thumbnail_tooltip_template");
    }
  }
}