using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Interface for polymorphic item type InBasket Task </summary>
  public interface IInBasketTask : IItem
  {
    /// <summary>Retrieve the <c>assigned_to</c> property of the item</summary>
    IProperty_Item AssignedTo();
    /// <summary>Retrieve the <c>container</c> property of the item</summary>
    IProperty_Item Container();
    /// <summary>Retrieve the <c>container_type_id</c> property of the item</summary>
    IProperty_Item ContainerTypeId();
    /// <summary>Retrieve the <c>due_date</c> property of the item</summary>
    IProperty_Date DueDate();
    /// <summary>Retrieve the <c>icon</c> property of the item</summary>
    IProperty_Text Icon();
    /// <summary>Retrieve the <c>instructions</c> property of the item</summary>
    IProperty_Text Instructions();
    /// <summary>Retrieve the <c>item</c> property of the item</summary>
    IProperty_Item Item();
    /// <summary>Retrieve the <c>item_type_id</c> property of the item</summary>
    IProperty_Item ItemTypeId();
    /// <summary>Retrieve the <c>language_code_filter</c> property of the item</summary>
    IProperty_Text LanguageCodeFilter();
    /// <summary>Retrieve the <c>my_assignment</c> property of the item</summary>
    IProperty_Boolean MyAssignment();
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    IProperty_Text NameProp();
    /// <summary>Retrieve the <c>start_date</c> property of the item</summary>
    IProperty_Date StartDate();
    /// <summary>Retrieve the <c>status</c> property of the item</summary>
    IProperty_Text Status();
  }
}