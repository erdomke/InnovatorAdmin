using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SecureMessageMarkup </summary>
  public class SecureMessageMarkup : Item
  {
    protected SecureMessageMarkup() { }
    public SecureMessageMarkup(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SecureMessageMarkup() { Innovator.Client.Item.AddNullItem<SecureMessageMarkup>(new SecureMessageMarkup { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>file_id</c> property of the item</summary>
    public IProperty_Text FileId()
    {
      return this.Property("file_id");
    }
    /// <summary>Retrieve the <c>file_selector_id</c> property of the item</summary>
    public IProperty_Text FileSelectorId()
    {
      return this.Property("file_selector_id");
    }
    /// <summary>Retrieve the <c>file_selector_type_id</c> property of the item</summary>
    public IProperty_Text FileSelectorTypeId()
    {
      return this.Property("file_selector_type_id");
    }
    /// <summary>Retrieve the <c>markup_data</c> property of the item</summary>
    public IProperty_Text MarkupData()
    {
      return this.Property("markup_data");
    }
    /// <summary>Retrieve the <c>markup_holder_config_id</c> property of the item</summary>
    public IProperty_Text MarkupHolderConfigId()
    {
      return this.Property("markup_holder_config_id");
    }
    /// <summary>Retrieve the <c>markup_holder_id</c> property of the item</summary>
    public IProperty_Text MarkupHolderId()
    {
      return this.Property("markup_holder_id");
    }
    /// <summary>Retrieve the <c>markup_holder_major_rev</c> property of the item</summary>
    public IProperty_Text MarkupHolderMajorRev()
    {
      return this.Property("markup_holder_major_rev");
    }
    /// <summary>Retrieve the <c>markup_holder_state</c> property of the item</summary>
    public IProperty_Text MarkupHolderState()
    {
      return this.Property("markup_holder_state");
    }
    /// <summary>Retrieve the <c>markup_holder_type_id</c> property of the item</summary>
    public IProperty_Text MarkupHolderTypeId()
    {
      return this.Property("markup_holder_type_id");
    }
    /// <summary>Retrieve the <c>markup_holder_version</c> property of the item</summary>
    public IProperty_Number MarkupHolderVersion()
    {
      return this.Property("markup_holder_version");
    }
    /// <summary>Retrieve the <c>snapshot</c> property of the item</summary>
    public IProperty_Text Snapshot()
    {
      return this.Property("snapshot");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>source_type</c> property of the item</summary>
    public IProperty_Item<ItemType> SourceType()
    {
      return this.Property("source_type");
    }
    /// <summary>Retrieve the <c>thumbnail</c> property of the item</summary>
    public IProperty_Text Thumbnail()
    {
      return this.Property("thumbnail");
    }
  }
}