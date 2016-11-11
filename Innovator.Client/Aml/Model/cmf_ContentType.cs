using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ContentType </summary>
  public class cmf_ContentType : Item
  {
    protected cmf_ContentType() { }
    public cmf_ContentType(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ContentType() { Innovator.Client.Item.AddNullItem<cmf_ContentType>(new cmf_ContentType { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>linked_item_type</c> property of the item</summary>
    public IProperty_Item<ItemType> LinkedItemType()
    {
      return this.Property("linked_item_type");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
  }
}