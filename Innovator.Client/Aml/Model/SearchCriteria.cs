using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Search Criteria </summary>
  public class SearchCriteria : Item, INullRelationship<Search>
  {
    protected SearchCriteria() { }
    public SearchCriteria(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SearchCriteria() { Innovator.Client.Item.AddNullItem<SearchCriteria>(new SearchCriteria { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>condition</c> property of the item</summary>
    public IProperty_Text Condition()
    {
      return this.Property("condition");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>property_name</c> property of the item</summary>
    public IProperty_Text PropertyName()
    {
      return this.Property("property_name");
    }
    /// <summary>Retrieve the <c>property_value</c> property of the item</summary>
    public IProperty_Text PropertyValue()
    {
      return this.Property("property_value");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}