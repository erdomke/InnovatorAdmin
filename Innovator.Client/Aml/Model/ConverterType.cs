using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ConverterType </summary>
  public class ConverterType : Item
  {
    protected ConverterType() { }
    public ConverterType(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ConverterType() { Innovator.Client.Item.AddNullItem<ConverterType>(new ConverterType { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
  }
}