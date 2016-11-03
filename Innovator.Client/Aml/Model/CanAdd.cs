using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Can Add </summary>
  public class CanAdd : Item, INullRelationship<ItemType>, IRelationship<Identity>
  {
    protected CanAdd() { }
    public CanAdd(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static CanAdd() { Innovator.Client.Item.AddNullItem<CanAdd>(new CanAdd { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>can_add</c> property of the item</summary>
    public IProperty_Boolean CanAddProp()
    {
      return this.Property("can_add");
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