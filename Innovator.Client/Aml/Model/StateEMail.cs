using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type State EMail </summary>
  public class StateEMail : Item
  {
    protected StateEMail() { }
    public StateEMail(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
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
    public IProperty_Item OfItemtype()
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