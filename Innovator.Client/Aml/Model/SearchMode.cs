using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SearchMode </summary>
  public class SearchMode : Item
  {
    protected SearchMode() { }
    public SearchMode(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SearchMode() { Innovator.Client.Item.AddNullItem<SearchMode>(new SearchMode { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>is_active</c> property of the item</summary>
    public IProperty_Boolean IsActive()
    {
      return this.Property("is_active");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>search_handler</c> property of the item</summary>
    public IProperty_Text SearchHandler()
    {
      return this.Property("search_handler");
    }
  }
}