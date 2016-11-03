using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Frame </summary>
  public class Frame : Item, INullRelationship<Frameset>
  {
    protected Frame() { }
    public Frame(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Frame() { Innovator.Client.Item.AddNullItem<Frame>(new Frame { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>color</c> property of the item</summary>
    public IProperty_Text Color()
    {
      return this.Property("color");
    }
    /// <summary>Retrieve the <c>frameborder</c> property of the item</summary>
    public IProperty_Text Frameborder()
    {
      return this.Property("frameborder");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>margin_height</c> property of the item</summary>
    public IProperty_Number MarginHeight()
    {
      return this.Property("margin_height");
    }
    /// <summary>Retrieve the <c>margin_width</c> property of the item</summary>
    public IProperty_Number MarginWidth()
    {
      return this.Property("margin_width");
    }
    /// <summary>Retrieve the <c>noresize</c> property of the item</summary>
    public IProperty_Boolean Noresize()
    {
      return this.Property("noresize");
    }
    /// <summary>Retrieve the <c>scrolling</c> property of the item</summary>
    public IProperty_Text Scrolling()
    {
      return this.Property("scrolling");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}