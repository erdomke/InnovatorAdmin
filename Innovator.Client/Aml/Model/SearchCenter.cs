using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Search Center </summary>
  public class SearchCenter : Item
  {
    protected SearchCenter() { }
    public SearchCenter(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SearchCenter() { Innovator.Client.Item.AddNullItem<SearchCenter>(new SearchCenter { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

  }
}