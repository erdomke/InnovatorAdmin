using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type GlobalPresentationConfig </summary>
  public class GlobalPresentationConfig : Item
  {
    protected GlobalPresentationConfig() { }
    public GlobalPresentationConfig(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>additional_data</c> property of the item</summary>
    public IProperty_Text AdditionalData()
    {
      return this.Property("additional_data");
    }
    /// <summary>Retrieve the <c>client</c> property of the item</summary>
    public IProperty_Text Client()
    {
      return this.Property("client");
    }
    /// <summary>Retrieve the <c>presentation_id</c> property of the item</summary>
    public IProperty_Item PresentationId()
    {
      return this.Property("presentation_id");
    }
    /// <summary>Retrieve the <c>view_type</c> property of the item</summary>
    public IProperty_Text ViewType()
    {
      return this.Property("view_type");
    }
  }
}