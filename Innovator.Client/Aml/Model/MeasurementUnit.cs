using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Measurement Unit </summary>
  public class MeasurementUnit : Item
  {
    protected MeasurementUnit() { }
    public MeasurementUnit(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>abbreviation</c> property of the item</summary>
    public IProperty_Text Abbreviation()
    {
      return this.Property("abbreviation");
    }
    /// <summary>Retrieve the <c>is_default</c> property of the item</summary>
    public IProperty_Boolean IsDefault()
    {
      return this.Property("is_default");
    }
    /// <summary>Retrieve the <c>ratio_to_inch</c> property of the item</summary>
    public IProperty_Number RatioToInch()
    {
      return this.Property("ratio_to_inch");
    }
    /// <summary>Retrieve the <c>sequence</c> property of the item</summary>
    public IProperty_Number Sequence()
    {
      return this.Property("sequence");
    }
    /// <summary>Retrieve the <c>unit</c> property of the item</summary>
    public IProperty_Text Unit()
    {
      return this.Property("unit");
    }
    /// <summary>Retrieve the <c>units</c> property of the item</summary>
    public IProperty_Text Units()
    {
      return this.Property("units");
    }
    /// <summary>Retrieve the <c>viewer</c> property of the item</summary>
    public IProperty_Item Viewer()
    {
      return this.Property("viewer");
    }
  }
}