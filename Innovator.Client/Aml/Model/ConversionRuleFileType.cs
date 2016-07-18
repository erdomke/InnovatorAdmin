using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ConversionRuleFileType </summary>
  public class ConversionRuleFileType : Item
  {
    protected ConversionRuleFileType() { }
    public ConversionRuleFileType(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>enabled</c> property of the item</summary>
    public IProperty_Boolean Enabled()
    {
      return this.Property("enabled");
    }
    /// <summary>Retrieve the <c>options</c> property of the item</summary>
    public IProperty_Text Options()
    {
      return this.Property("options");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}