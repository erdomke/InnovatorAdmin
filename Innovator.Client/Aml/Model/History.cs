using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type History </summary>
  public class History : Item, INullRelationship<HistoryContainer>
  {
    protected History() { }
    public History(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static History() { Innovator.Client.Item.AddNullItem<History>(new History { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>action</c> property of the item</summary>
    public IProperty_Text Action()
    {
      return this.Property("action");
    }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>comments</c> property of the item</summary>
    public IProperty_Text Comments()
    {
      return this.Property("comments");
    }
    /// <summary>Retrieve the <c>created_on_tick</c> property of the item</summary>
    public IProperty_Number CreatedOnTick()
    {
      return this.Property("created_on_tick");
    }
    /// <summary>Retrieve the <c>item_id</c> property of the item</summary>
    public IProperty_Text ItemId()
    {
      return this.Property("item_id");
    }
    /// <summary>Retrieve the <c>item_major_rev</c> property of the item</summary>
    public IProperty_Text ItemMajorRev()
    {
      return this.Property("item_major_rev");
    }
    /// <summary>Retrieve the <c>item_state</c> property of the item</summary>
    public IProperty_Text ItemState()
    {
      return this.Property("item_state");
    }
    /// <summary>Retrieve the <c>item_version</c> property of the item</summary>
    public IProperty_Number ItemVersion()
    {
      return this.Property("item_version");
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
    /// <summary>Retrieve the <c>version</c> property of the item</summary>
    public IProperty_Number Version()
    {
      return this.Property("version");
    }
  }
}