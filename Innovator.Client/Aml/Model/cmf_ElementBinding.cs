using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ElementBinding </summary>
  public class cmf_ElementBinding : Item, INullRelationship<cmf_ElementType>
  {
    protected cmf_ElementBinding() { }
    public cmf_ElementBinding(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ElementBinding() { Innovator.Client.Item.AddNullItem<cmf_ElementBinding>(new cmf_ElementBinding { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>item_delete_behavior</c> property of the item</summary>
    public IProperty_Text ItemDeleteBehavior()
    {
      return this.Property("item_delete_behavior");
    }
    /// <summary>Retrieve the <c>new_row_mode</c> property of the item</summary>
    public IProperty_Text NewRowMode()
    {
      return this.Property("new_row_mode");
    }
    /// <summary>Retrieve the <c>on_after_pick</c> property of the item</summary>
    public IProperty_Item<Method> OnAfterPick()
    {
      return this.Property("on_after_pick");
    }
    /// <summary>Retrieve the <c>on_apply_binding</c> property of the item</summary>
    public IProperty_Item<Method> OnApplyBinding()
    {
      return this.Property("on_apply_binding");
    }
    /// <summary>Retrieve the <c>on_create_reference</c> property of the item</summary>
    public IProperty_Item<Method> OnCreateReference()
    {
      return this.Property("on_create_reference");
    }
    /// <summary>Retrieve the <c>reference_required</c> property of the item</summary>
    public IProperty_Boolean ReferenceRequired()
    {
      return this.Property("reference_required");
    }
    /// <summary>Retrieve the <c>reference_type</c> property of the item</summary>
    public IProperty_Item<ItemType> ReferenceType()
    {
      return this.Property("reference_type");
    }
    /// <summary>Retrieve the <c>resolution_mode</c> property of the item</summary>
    public IProperty_Text ResolutionMode()
    {
      return this.Property("resolution_mode");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>structure_mapping_method</c> property of the item</summary>
    public IProperty_Item<Method> StructureMappingMethod()
    {
      return this.Property("structure_mapping_method");
    }
    /// <summary>Retrieve the <c>synchronization_direction</c> property of the item</summary>
    public IProperty_Text SynchronizationDirection()
    {
      return this.Property("synchronization_direction");
    }
    /// <summary>Retrieve the <c>tracking_mode</c> property of the item</summary>
    public IProperty_Text TrackingMode()
    {
      return this.Property("tracking_mode");
    }
  }
}