using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_TabularViewHeaderRow </summary>
  public class cmf_TabularViewHeaderRow : Item
  {
    protected cmf_TabularViewHeaderRow() { }
    public cmf_TabularViewHeaderRow(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>group_level</c> property of the item</summary>
    public IProperty_Number GroupLevel()
    {
      return this.Property("group_level");
    }
    /// <summary>Retrieve the <c>header_style</c> property of the item</summary>
    public IProperty_Item HeaderStyle()
    {
      return this.Property("header_style");
    }
  }
}