using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Frameset </summary>
  public class Frameset : Item
  {
    protected Frameset() { }
    public Frameset(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>cols</c> property of the item</summary>
    public IProperty_Text Cols()
    {
      return this.Property("cols");
    }
    /// <summary>Retrieve the <c>frameborder</c> property of the item</summary>
    public IProperty_Text Frameborder()
    {
      return this.Property("frameborder");
    }
    /// <summary>Retrieve the <c>framespacing</c> property of the item</summary>
    public IProperty_Number Framespacing()
    {
      return this.Property("framespacing");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>rows_</c> property of the item</summary>
    public IProperty_Text Rows()
    {
      return this.Property("rows_");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}