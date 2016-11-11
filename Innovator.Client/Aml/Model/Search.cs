using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Search </summary>
  public class Search : Item
  {
    protected Search() { }
    public Search(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Search() { Innovator.Client.Item.AddNullItem<Search>(new Search { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>interactive</c> property of the item</summary>
    public IProperty_Boolean Interactive()
    {
      return this.Property("interactive");
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
    /// <summary>Retrieve the <c>search_itemtype</c> property of the item</summary>
    public IProperty_Item<ItemType> SearchItemtype()
    {
      return this.Property("search_itemtype");
    }
  }
}