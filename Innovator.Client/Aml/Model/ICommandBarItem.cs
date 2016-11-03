using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Interface for polymorphic item type CommandBarItem </summary>
  public interface ICommandBarItem : IItem
  {
    /// <summary>Retrieve the <c>additional_data</c> property of the item</summary>
    IProperty_Text AdditionalData();
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    IProperty_Text NameProp();
    /// <summary>Retrieve the <c>on_init_handler</c> property of the item</summary>
    IProperty_Item<Method> OnInitHandler();
  }
}