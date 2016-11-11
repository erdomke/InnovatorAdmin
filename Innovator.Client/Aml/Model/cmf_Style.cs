using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_Style </summary>
  public class cmf_Style : Item
  {
    protected cmf_Style() { }
    public cmf_Style(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_Style() { Innovator.Client.Item.AddNullItem<cmf_Style>(new cmf_Style { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>background_color</c> property of the item</summary>
    public IProperty_Text BackgroundColor()
    {
      return this.Property("background_color");
    }
    /// <summary>Retrieve the <c>font_family</c> property of the item</summary>
    public IProperty_Text FontFamily()
    {
      return this.Property("font_family");
    }
    /// <summary>Retrieve the <c>font_size</c> property of the item</summary>
    public IProperty_Number FontSize()
    {
      return this.Property("font_size");
    }
    /// <summary>Retrieve the <c>font_style</c> property of the item</summary>
    public IProperty_Text FontStyle()
    {
      return this.Property("font_style");
    }
    /// <summary>Retrieve the <c>font_weight</c> property of the item</summary>
    public IProperty_Text FontWeight()
    {
      return this.Property("font_weight");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>text_align</c> property of the item</summary>
    public IProperty_Text TextAlign()
    {
      return this.Property("text_align");
    }
    /// <summary>Retrieve the <c>text_color</c> property of the item</summary>
    public IProperty_Text TextColor()
    {
      return this.Property("text_color");
    }
    /// <summary>Retrieve the <c>text_decoration</c> property of the item</summary>
    public IProperty_Text TextDecoration()
    {
      return this.Property("text_decoration");
    }
  }
}