using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SSVCItems </summary>
  public class SSVCItems : Item, ISSVCItems
  {
    protected SSVCItems() { }
    public SSVCItems(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SSVCItems() { Innovator.Client.Item.AddNullItem<SSVCItems>(new SSVCItems { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

  }
}
