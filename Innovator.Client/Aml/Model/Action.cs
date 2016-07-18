using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Action </summary>
  public class Action : Item
  {
    protected Action() { }
    public Action(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>body</c> property of the item</summary>
    public IProperty_Text Body()
    {
      return this.Property("body");
    }
    /// <summary>Retrieve the <c>item_query</c> property of the item</summary>
    public IProperty_Text ItemQuery()
    {
      return this.Property("item_query");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>location</c> property of the item</summary>
    public IProperty_Text Location()
    {
      return this.Property("location");
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
    /// <summary>Retrieve the <c>on_complete</c> property of the item</summary>
    public IProperty_Item OnComplete()
    {
      return this.Property("on_complete");
    }
    /// <summary>Retrieve the <c>target</c> property of the item</summary>
    public IProperty_Text Target()
    {
      return this.Property("target");
    }
    /// <summary>Retrieve the <c>type</c> property of the item</summary>
    public IProperty_Text Type()
    {
      return this.Property("type");
    }
  }
}