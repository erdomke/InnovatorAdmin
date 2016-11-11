using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type WSTypeProperty </summary>
  public class WSTypeProperty : Item, INullRelationship<WSType>
  {
    protected WSTypeProperty() { }
    public WSTypeProperty(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static WSTypeProperty() { Innovator.Client.Item.AddNullItem<WSTypeProperty>(new WSTypeProperty { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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
    /// <summary>Retrieve the <c>property_name</c> property of the item</summary>
    public IProperty_Text PropertyName()
    {
      return this.Property("property_name");
    }
    /// <summary>Retrieve the <c>property_real_type</c> property of the item</summary>
    public IProperty_Text PropertyRealType()
    {
      return this.Property("property_real_type");
    }
    /// <summary>Retrieve the <c>property_type</c> property of the item</summary>
    public IProperty_Text PropertyType()
    {
      return this.Property("property_type");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}