using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ItemType Life Cycle </summary>
  public class ItemTypeLifeCycle : Item, INullRelationship<ItemType>, IRelationship<LifeCycleMap>
  {
    protected ItemTypeLifeCycle() { }
    public ItemTypeLifeCycle(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ItemTypeLifeCycle() { Innovator.Client.Item.AddNullItem<ItemTypeLifeCycle>(new ItemTypeLifeCycle { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>class_path</c> property of the item</summary>
    public IProperty_Text ClassPath()
    {
      return this.Property("class_path");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}