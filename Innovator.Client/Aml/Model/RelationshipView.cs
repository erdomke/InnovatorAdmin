using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Relationship View </summary>
  public class RelationshipView : Item
  {
    protected RelationshipView() { }
    public RelationshipView(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>form</c> property of the item</summary>
    public IProperty_Item Form()
    {
      return this.Property("form");
    }
    /// <summary>Retrieve the <c>grid</c> property of the item</summary>
    public IProperty_Item Grid()
    {
      return this.Property("grid");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>parameters</c> property of the item</summary>
    public IProperty_Text Parameters()
    {
      return this.Property("parameters");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>start_page</c> property of the item</summary>
    public IProperty_Text StartPage()
    {
      return this.Property("start_page");
    }
  }
}