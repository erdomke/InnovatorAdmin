using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type CommandBarItem </summary>
  public class CommandBarItem : Item, ICommandBarItem
  {
    protected CommandBarItem() { }
    public CommandBarItem(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static CommandBarItem() { Innovator.Client.Item.AddNullItem<CommandBarItem>(new CommandBarItem { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>additional_data</c> property of the item</summary>
    public IProperty_Text AdditionalData()
    {
      return this.Property("additional_data");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>on_init_handler</c> property of the item</summary>
    public IProperty_Item<Method> OnInitHandler()
    {
      return this.Property("on_init_handler");
    }
  }
}