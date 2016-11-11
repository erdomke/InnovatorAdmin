using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Exclusion </summary>
  public class Exclusion : Item, INullRelationship<RelationshipType>, IRelationship<RelationshipType>
  {
    protected Exclusion() { }
    public Exclusion(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Exclusion() { Innovator.Client.Item.AddNullItem<Exclusion>(new Exclusion { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}