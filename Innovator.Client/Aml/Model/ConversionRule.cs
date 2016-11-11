using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ConversionRule </summary>
  public class ConversionRule : Item
  {
    protected ConversionRule() { }
    public ConversionRule(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ConversionRule() { Innovator.Client.Item.AddNullItem<ConversionRule>(new ConversionRule { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>converter_type</c> property of the item</summary>
    public IProperty_Item<ConverterType> ConverterType()
    {
      return this.Property("converter_type");
    }
    /// <summary>Retrieve the <c>cutoff</c> property of the item</summary>
    public IProperty_Number Cutoff()
    {
      return this.Property("cutoff");
    }
    /// <summary>Retrieve the <c>delay</c> property of the item</summary>
    public IProperty_Number Delay()
    {
      return this.Property("delay");
    }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>enabled</c> property of the item</summary>
    public IProperty_Boolean Enabled()
    {
      return this.Property("enabled");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>timeout</c> property of the item</summary>
    public IProperty_Number Timeout()
    {
      return this.Property("timeout");
    }
  }
}