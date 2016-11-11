using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type WSType </summary>
  public class WSType : Item, INullRelationship<WSConfiguration>
  {
    protected WSType() { }
    public WSType(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static WSType() { Innovator.Client.Item.AddNullItem<WSType>(new WSType { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>alias</c> property of the item</summary>
    public IProperty_Text Alias()
    {
      return this.Property("alias");
    }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>expand</c> property of the item</summary>
    public IProperty_Boolean Expand()
    {
      return this.Property("expand");
    }
    /// <summary>Retrieve the <c>is_top</c> property of the item</summary>
    public IProperty_Boolean IsTop()
    {
      return this.Property("is_top");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>type_name</c> property of the item</summary>
    public IProperty_Text TypeNameProp()
    {
      return this.Property("type_name");
    }
  }
}