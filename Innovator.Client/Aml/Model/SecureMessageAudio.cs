using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SecureMessageAudio </summary>
  public class SecureMessageAudio : Item
  {
    protected SecureMessageAudio() { }
    public SecureMessageAudio(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>media_file</c> property of the item</summary>
    public IProperty_Item MediaFile()
    {
      return this.Property("media_file");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>source_type</c> property of the item</summary>
    public IProperty_Item SourceType()
    {
      return this.Property("source_type");
    }
  }
}