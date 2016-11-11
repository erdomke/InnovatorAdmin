using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SecureMessageVideo </summary>
  public class SecureMessageVideo : Item, IFileContainerItems
  {
    protected SecureMessageVideo() { }
    public SecureMessageVideo(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SecureMessageVideo() { Innovator.Client.Item.AddNullItem<SecureMessageVideo>(new SecureMessageVideo { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>media_file</c> property of the item</summary>
    public IProperty_Item<File> MediaFile()
    {
      return this.Property("media_file");
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
  }
}