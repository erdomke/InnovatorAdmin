using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type PreferenceTabTypes </summary>
  public class PreferenceTabTypes : Item, INullRelationship<PreferenceTypes>
  {
    protected PreferenceTabTypes() { }
    public PreferenceTabTypes(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static PreferenceTabTypes() { Innovator.Client.Item.AddNullItem<PreferenceTabTypes>(new PreferenceTabTypes { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>preference_tab</c> property of the item</summary>
    public IProperty_Text PreferenceTab()
    {
      return this.Property("preference_tab");
    }
    /// <summary>Retrieve the <c>preference_tab_type</c> property of the item</summary>
    public IProperty_Text PreferenceTabType()
    {
      return this.Property("preference_tab_type");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}