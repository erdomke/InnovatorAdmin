using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type HistorySecureMessage </summary>
  public class HistorySecureMessage : Item
  {
    protected HistorySecureMessage() { }
    public HistorySecureMessage(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static HistorySecureMessage() { Innovator.Client.Item.AddNullItem<HistorySecureMessage>(new HistorySecureMessage { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>comments</c> property of the item</summary>
    public IProperty_Text Comments()
    {
      return this.Property("comments");
    }
    /// <summary>Retrieve the <c>disabled_by_id</c> property of the item</summary>
    public IProperty_Item<User> DisabledById()
    {
      return this.Property("disabled_by_id");
    }
    /// <summary>Retrieve the <c>disabled_on</c> property of the item</summary>
    public IProperty_Date DisabledOn()
    {
      return this.Property("disabled_on");
    }
    /// <summary>Retrieve the <c>item_config_id</c> property of the item</summary>
    public IProperty_Text ItemConfigId()
    {
      return this.Property("item_config_id");
    }
    /// <summary>Retrieve the <c>item_id</c> property of the item</summary>
    public IProperty_Text ItemId()
    {
      return this.Property("item_id");
    }
    /// <summary>Retrieve the <c>item_keyed_name</c> property of the item</summary>
    public IProperty_Text ItemKeyedName()
    {
      return this.Property("item_keyed_name");
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
    /// <summary>Retrieve the <c>item_type_name</c> property of the item</summary>
    public IProperty_Text ItemTypeName()
    {
      return this.Property("item_type_name");
    }
    /// <summary>Retrieve the <c>item_version</c> property of the item</summary>
    public IProperty_Number ItemVersion()
    {
      return this.Property("item_version");
    }
    /// <summary>Retrieve the <c>reply_to_id</c> property of the item</summary>
    public IProperty_Text ReplyToId()
    {
      return this.Property("reply_to_id");
    }
    /// <summary>Retrieve the <c>sm_data</c> property of the item</summary>
    public IProperty_Text SmData()
    {
      return this.Property("sm_data");
    }
    /// <summary>Retrieve the <c>thread_id</c> property of the item</summary>
    public IProperty_Text ThreadId()
    {
      return this.Property("thread_id");
    }
  }
}