using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SelfServiceReportFeatured </summary>
  public class SelfServiceReportFeatured : Item, INullRelationship<SelfServiceReportSettings>
  {
    protected SelfServiceReportFeatured() { }
    public SelfServiceReportFeatured(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SelfServiceReportFeatured() { Innovator.Client.Item.AddNullItem<SelfServiceReportFeatured>(new SelfServiceReportFeatured { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>allow_excluded_properties</c> property of the item</summary>
    public IProperty_Boolean AllowExcludedProperties()
    {
      return this.Property("allow_excluded_properties");
    }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>excluded_properties</c> property of the item</summary>
    public IProperty_Text ExcludedProperties()
    {
      return this.Property("excluded_properties");
    }
    /// <summary>Retrieve the <c>featured_itemtypes</c> property of the item</summary>
    public IProperty_Text FeaturedItemtypes()
    {
      return this.Property("featured_itemtypes");
    }
    /// <summary>Retrieve the <c>hide_advanced</c> property of the item</summary>
    public IProperty_Boolean HideAdvanced()
    {
      return this.Property("hide_advanced");
    }
    /// <summary>Retrieve the <c>identity_id</c> property of the item</summary>
    public IProperty_Item<Identity> IdentityId()
    {
      return this.Property("identity_id");
    }
    /// <summary>Retrieve the <c>show_all</c> property of the item</summary>
    public IProperty_Boolean ShowAll()
    {
      return this.Property("show_all");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}