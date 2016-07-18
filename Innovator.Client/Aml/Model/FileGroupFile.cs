using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type FileGroup File </summary>
  public class FileGroupFile : Item
  {
    protected FileGroupFile() { }
    public FileGroupFile(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>file_expiration_date</c> property of the item</summary>
    public IProperty_Date FileExpirationDate()
    {
      return this.Property("file_expiration_date");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}