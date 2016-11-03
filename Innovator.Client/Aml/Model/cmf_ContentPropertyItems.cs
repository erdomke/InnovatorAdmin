using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ContentPropertyItems </summary>
  public class cmf_ContentPropertyItems : Item, Icmf_ContentPropertyItems
  {
    protected cmf_ContentPropertyItems() { }
    public cmf_ContentPropertyItems(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ContentPropertyItems() { Innovator.Client.Item.AddNullItem<cmf_ContentPropertyItems>(new cmf_ContentPropertyItems { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

  }
}