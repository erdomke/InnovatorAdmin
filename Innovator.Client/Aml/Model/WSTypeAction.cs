using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type WSTypeAction </summary>
  public class WSTypeAction : Item, INullRelationship<WSType>
  {
    protected WSTypeAction() { }
    public WSTypeAction(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static WSTypeAction() { Innovator.Client.Item.AddNullItem<WSTypeAction>(new WSTypeAction { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>custom_action_name</c> property of the item</summary>
    public IProperty_Text CustomActionName()
    {
      return this.Property("custom_action_name");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}