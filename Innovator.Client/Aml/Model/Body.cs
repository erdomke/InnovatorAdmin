using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Body </summary>
  public class Body : Item, INullRelationship<Form>
  {
    protected Body() { }
    public Body(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Body() { Innovator.Client.Item.AddNullItem<Body>(new Body { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>bg_attach</c> property of the item</summary>
    public IProperty_Text BgAttach()
    {
      return this.Property("bg_attach");
    }
    /// <summary>Retrieve the <c>bg_color</c> property of the item</summary>
    public IProperty_Text BgColor()
    {
      return this.Property("bg_color");
    }
    /// <summary>Retrieve the <c>bg_image</c> property of the item</summary>
    public IProperty_Text BgImage()
    {
      return this.Property("bg_image");
    }
    /// <summary>Retrieve the <c>bg_repeat</c> property of the item</summary>
    public IProperty_Text BgRepeat()
    {
      return this.Property("bg_repeat");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}