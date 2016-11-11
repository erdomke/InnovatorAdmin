using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ReadPriority </summary>
  public class ReadPriority : Item, INullRelationship<User>, IRelationship<Vault>
  {
    protected ReadPriority() { }
    public ReadPriority(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ReadPriority() { Innovator.Client.Item.AddNullItem<ReadPriority>(new ReadPriority { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>priority</c> property of the item</summary>
    public IProperty_Number Priority()
    {
      return this.Property("priority");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}