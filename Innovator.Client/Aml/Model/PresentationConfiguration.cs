using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type PresentationConfiguration </summary>
  public class PresentationConfiguration : Item
  {
    protected PresentationConfiguration() { }
    public PresentationConfiguration(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static PresentationConfiguration() { Innovator.Client.Item.AddNullItem<PresentationConfiguration>(new PresentationConfiguration { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>secondary_icon</c> property of the item</summary>
    public IProperty_Text SecondaryIcon()
    {
      return this.Property("secondary_icon");
    }
    /// <summary>Retrieve the <c>style</c> property of the item</summary>
    public IProperty_Text Style()
    {
      return this.Property("style");
    }
  }
}