using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Life Cycle Map </summary>
  public class LifeCycleMap : Item
  {
    protected LifeCycleMap() { }
    public LifeCycleMap(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static LifeCycleMap() { Innovator.Client.Item.AddNullItem<LifeCycleMap>(new LifeCycleMap { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
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
    /// <summary>Retrieve the <c>start_state</c> property of the item</summary>
    public IProperty_Item<LifeCycleState> StartState()
    {
      return this.Property("start_state");
    }
  }
}