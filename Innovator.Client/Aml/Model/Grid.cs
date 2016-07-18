using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Grid </summary>
  public class Grid : Item
  {
    protected Grid() { }
    public Grid(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>merge_path</c> property of the item</summary>
    public IProperty_Text MergePath()
    {
      return this.Property("merge_path");
    }
    /// <summary>Retrieve the <c>method</c> property of the item</summary>
    public IProperty_Item Method()
    {
      return this.Property("method");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>query</c> property of the item</summary>
    public IProperty_Text Query()
    {
      return this.Property("query");
    }
  }
}