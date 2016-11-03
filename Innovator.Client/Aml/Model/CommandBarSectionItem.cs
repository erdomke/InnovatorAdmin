using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type CommandBarSectionItem </summary>
  public class CommandBarSectionItem : Item, INullRelationship<CommandBarSection>, IRelationship<CommandBarItem>
  {
    protected CommandBarSectionItem() { }
    public CommandBarSectionItem(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static CommandBarSectionItem() { Innovator.Client.Item.AddNullItem<CommandBarSectionItem>(new CommandBarSectionItem { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>action</c> property of the item</summary>
    public IProperty_Text Action()
    {
      return this.Property("action");
    }
    /// <summary>Retrieve the <c>alternate</c> property of the item</summary>
    public IProperty_Item<CommandBarItem> Alternate()
    {
      return this.Property("alternate");
    }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>item_classification</c> property of the item</summary>
    public IProperty_Text ItemClassification()
    {
      return this.Property("item_classification");
    }
    /// <summary>Retrieve the <c>role</c> property of the item</summary>
    public IProperty_Item<Identity> Role()
    {
      return this.Property("role");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}