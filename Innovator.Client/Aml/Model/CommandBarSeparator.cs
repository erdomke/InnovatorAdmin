using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type CommandBarSeparator </summary>
  public class CommandBarSeparator : Item, ICommandBarItem
  {
    protected CommandBarSeparator() { }
    public CommandBarSeparator(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>additional_data</c> property of the item</summary>
    public IProperty_Text AdditionalData()
    {
      return this.Property("additional_data");
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
    /// <summary>Retrieve the <c>on_init_handler</c> property of the item</summary>
    public IProperty_Item OnInitHandler()
    {
      return this.Property("on_init_handler");
    }
  }
}