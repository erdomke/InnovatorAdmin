using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ConversionServerPriority </summary>
  public class ConversionServerPriority : Item, INullRelationship<Vault>, IRelationship<ConversionServer>
  {
    protected ConversionServerPriority() { }
    public ConversionServerPriority(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ConversionServerPriority() { Innovator.Client.Item.AddNullItem<ConversionServerPriority>(new ConversionServerPriority { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}