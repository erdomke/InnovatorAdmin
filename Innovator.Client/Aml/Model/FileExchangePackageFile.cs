using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type FileExchangePackageFile </summary>
  public class FileExchangePackageFile : Item
  {
    protected FileExchangePackageFile() { }
    public FileExchangePackageFile(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>parental_id</c> property of the item</summary>
    public IProperty_Text ParentalId()
    {
      return this.Property("parental_id");
    }
    /// <summary>Retrieve the <c>parental_property</c> property of the item</summary>
    public IProperty_Text ParentalProperty()
    {
      return this.Property("parental_property");
    }
    /// <summary>Retrieve the <c>parental_type</c> property of the item</summary>
    public IProperty_Text ParentalType()
    {
      return this.Property("parental_type");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}