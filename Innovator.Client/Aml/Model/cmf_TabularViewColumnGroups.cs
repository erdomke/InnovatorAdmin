using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_TabularViewColumnGroups </summary>
  public class cmf_TabularViewColumnGroups : Item, INullRelationship<cmf_TabularViewHeaderRow>
  {
    protected cmf_TabularViewColumnGroups() { }
    public cmf_TabularViewColumnGroups(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_TabularViewColumnGroups() { Innovator.Client.Item.AddNullItem<cmf_TabularViewColumnGroups>(new cmf_TabularViewColumnGroups { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>end_column</c> property of the item</summary>
    public IProperty_Number EndColumn()
    {
      return this.Property("end_column");
    }
    /// <summary>Retrieve the <c>group_style</c> property of the item</summary>
    public IProperty_Item<cmf_Style> GroupStyle()
    {
      return this.Property("group_style");
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
    /// <summary>Retrieve the <c>start_column</c> property of the item</summary>
    public IProperty_Number StartColumn()
    {
      return this.Property("start_column");
    }
  }
}