using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type TOC Access </summary>
  public class TOCAccess : Item, INullRelationship<ItemType>, IRelationship<Identity>
  {
    protected TOCAccess() { }
    public TOCAccess(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static TOCAccess() { Innovator.Client.Item.AddNullItem<TOCAccess>(new TOCAccess { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>category</c> property of the item</summary>
    public IProperty_Text Category()
    {
      return this.Property("category");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}