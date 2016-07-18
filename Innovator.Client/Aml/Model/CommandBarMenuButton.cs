using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type CommandBarMenuButton </summary>
  public class CommandBarMenuButton : Item, ICommandBarItem
  {
    protected CommandBarMenuButton() { }
    public CommandBarMenuButton(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>additional_data</c> property of the item</summary>
    public IProperty_Text AdditionalData()
    {
      return this.Property("additional_data");
    }
    /// <summary>Retrieve the <c>image</c> property of the item</summary>
    public IProperty_Text Image()
    {
      return this.Property("image");
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
    /// <summary>Retrieve the <c>on_click_handler</c> property of the item</summary>
    public IProperty_Item OnClickHandler()
    {
      return this.Property("on_click_handler");
    }
    /// <summary>Retrieve the <c>on_init_handler</c> property of the item</summary>
    public IProperty_Item OnInitHandler()
    {
      return this.Property("on_init_handler");
    }
    /// <summary>Retrieve the <c>parent_menu</c> property of the item</summary>
    public IProperty_Item ParentMenu()
    {
      return this.Property("parent_menu");
    }
  }
}