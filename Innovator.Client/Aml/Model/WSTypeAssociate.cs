using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type WSTypeAssociate </summary>
  public class WSTypeAssociate : Item, INullRelationship<WSType>, IRelationship<WSType>
  {
    protected WSTypeAssociate() { }
    public WSTypeAssociate(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static WSTypeAssociate() { Innovator.Client.Item.AddNullItem<WSTypeAssociate>(new WSTypeAssociate { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>ptiname</c> property of the item</summary>
    public IProperty_Text Ptiname()
    {
      return this.Property("ptiname");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}