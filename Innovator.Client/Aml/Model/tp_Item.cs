using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type tp_Item </summary>
  public class tp_Item : Item, Itp_Item
  {
    protected tp_Item() { }
    public tp_Item(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static tp_Item() { Innovator.Client.Item.AddNullItem<tp_Item>(new tp_Item { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

  }
}