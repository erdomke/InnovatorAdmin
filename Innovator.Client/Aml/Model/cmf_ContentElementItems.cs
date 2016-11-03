using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ContentElementItems </summary>
  public class cmf_ContentElementItems : Item, Icmf_ContentElementItems
  {
    protected cmf_ContentElementItems() { }
    public cmf_ContentElementItems(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ContentElementItems() { Innovator.Client.Item.AddNullItem<cmf_ContentElementItems>(new cmf_ContentElementItems { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

  }
}