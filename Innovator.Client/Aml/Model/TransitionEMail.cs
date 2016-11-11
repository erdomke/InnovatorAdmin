using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Transition EMail </summary>
  public class TransitionEMail : Item, INullRelationship<LifeCycleTransition>, IRelationship<EMailMessage>
  {
    protected TransitionEMail() { }
    public TransitionEMail(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static TransitionEMail() { Innovator.Client.Item.AddNullItem<TransitionEMail>(new TransitionEMail { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>of_itemtype</c> property of the item</summary>
    public IProperty_Item<ItemType> OfItemtype()
    {
      return this.Property("of_itemtype");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}