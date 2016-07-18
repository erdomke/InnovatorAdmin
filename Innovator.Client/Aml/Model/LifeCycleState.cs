using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Life Cycle State </summary>
  public class LifeCycleState : Item
  {
    protected LifeCycleState() { }
    public LifeCycleState(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>history_template</c> property of the item</summary>
    public IProperty_Item HistoryTemplate()
    {
      return this.Property("history_template");
    }
    /// <summary>Retrieve the <c>image</c> property of the item</summary>
    public IProperty_Text Image()
    {
      return this.Property("image");
    }
    /// <summary>Retrieve the <c>item_behavior</c> property of the item</summary>
    public IProperty_Text ItemBehavior()
    {
      return this.Property("item_behavior");
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
    /// <summary>Retrieve the <c>set_is_released</c> property of the item</summary>
    public IProperty_Boolean SetIsReleased()
    {
      return this.Property("set_is_released");
    }
    /// <summary>Retrieve the <c>set_not_lockable</c> property of the item</summary>
    public IProperty_Boolean SetNotLockable()
    {
      return this.Property("set_not_lockable");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>state_permission_id</c> property of the item</summary>
    public IProperty_Item StatePermissionId()
    {
      return this.Property("state_permission_id");
    }
    /// <summary>Retrieve the <c>workflow</c> property of the item</summary>
    public IProperty_Item Workflow()
    {
      return this.Property("workflow");
    }
    /// <summary>Retrieve the <c>x</c> property of the item</summary>
    public IProperty_Number X()
    {
      return this.Property("x");
    }
    /// <summary>Retrieve the <c>y</c> property of the item</summary>
    public IProperty_Number Y()
    {
      return this.Property("y");
    }
  }
}