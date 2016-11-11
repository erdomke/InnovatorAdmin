using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ConversionServerConverterType </summary>
  public class ConversionServerConverterType : Item, INullRelationship<ConversionServer>, IRelationship<ConverterType>
  {
    protected ConversionServerConverterType() { }
    public ConversionServerConverterType(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ConversionServerConverterType() { Innovator.Client.Item.AddNullItem<ConversionServerConverterType>(new ConversionServerConverterType { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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