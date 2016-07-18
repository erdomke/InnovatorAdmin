using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_AdditionalPropertyType </summary>
  public class cmf_AdditionalPropertyType : Item
  {
    protected cmf_AdditionalPropertyType() { }
    public cmf_AdditionalPropertyType(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>additional_property</c> property of the item</summary>
    public IProperty_Item AdditionalProperty()
    {
      return this.Property("additional_property");
    }
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