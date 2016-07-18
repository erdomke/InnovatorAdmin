using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_DocumentLifeCycleState </summary>
  public class cmf_DocumentLifeCycleState : Item
  {
    protected cmf_DocumentLifeCycleState() { }
    public cmf_DocumentLifeCycleState(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>life_cycle_state_id</c> property of the item</summary>
    public IProperty_Item LifeCycleStateId()
    {
      return this.Property("life_cycle_state_id");
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
    /// <summary>Retrieve the <c>tracking_mode</c> property of the item</summary>
    public IProperty_Text TrackingMode()
    {
      return this.Property("tracking_mode");
    }
  }
}