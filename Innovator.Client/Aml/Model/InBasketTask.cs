using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type InBasket Task </summary>
  public class InBasketTask : Item, IInBasketTask
  {
    protected InBasketTask() { }
    public InBasketTask(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static InBasketTask() { Innovator.Client.Item.AddNullItem<InBasketTask>(new InBasketTask { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>assigned_to</c> property of the item</summary>
    public IProperty_Item<Identity> AssignedTo()
    {
      return this.Property("assigned_to");
    }
    /// <summary>Retrieve the <c>container</c> property of the item</summary>
    public IProperty_Item<IReadOnlyItem> Container()
    {
      return this.Property("container");
    }
    /// <summary>Retrieve the <c>container_type_id</c> property of the item</summary>
    public IProperty_Item<ItemType> ContainerTypeId()
    {
      return this.Property("container_type_id");
    }
    /// <summary>Retrieve the <c>due_date</c> property of the item</summary>
    public IProperty_Date DueDate()
    {
      return this.Property("due_date");
    }
    /// <summary>Retrieve the <c>icon</c> property of the item</summary>
    public IProperty_Text Icon()
    {
      return this.Property("icon");
    }
    /// <summary>Retrieve the <c>instructions</c> property of the item</summary>
    public IProperty_Text Instructions()
    {
      return this.Property("instructions");
    }
    /// <summary>Retrieve the <c>item</c> property of the item</summary>
    public IProperty_Item<IReadOnlyItem> Item()
    {
      return this.Property("item");
    }
    /// <summary>Retrieve the <c>item_type_id</c> property of the item</summary>
    public IProperty_Item<ItemType> ItemTypeId()
    {
      return this.Property("item_type_id");
    }
    /// <summary>Retrieve the <c>language_code_filter</c> property of the item</summary>
    public IProperty_Text LanguageCodeFilter()
    {
      return this.Property("language_code_filter");
    }
    /// <summary>Retrieve the <c>my_assignment</c> property of the item</summary>
    public IProperty_Boolean MyAssignment()
    {
      return this.Property("my_assignment");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>start_date</c> property of the item</summary>
    public IProperty_Date StartDate()
    {
      return this.Property("start_date");
    }
    /// <summary>Retrieve the <c>status</c> property of the item</summary>
    public IProperty_Text Status()
    {
      return this.Property("status");
    }
  }
}