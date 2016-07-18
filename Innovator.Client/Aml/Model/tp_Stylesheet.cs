using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type tp_Stylesheet </summary>
  public class tp_Stylesheet : Item
  {
    protected tp_Stylesheet() { }
    public tp_Stylesheet(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>parent_stylesheet</c> property of the item</summary>
    public IProperty_Item ParentStylesheet()
    {
      return this.Property("parent_stylesheet");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>style_content</c> property of the item</summary>
    public IProperty_Text StyleContent()
    {
      return this.Property("style_content");
    }
  }
}