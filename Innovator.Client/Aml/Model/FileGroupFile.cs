using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type FileGroup File </summary>
  public class FileGroupFile : Item, IFileContainerItems, INullRelationship<FileGroup>, IRelationship<File>
  {
    protected FileGroupFile() { }
    public FileGroupFile(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static FileGroupFile() { Innovator.Client.Item.AddNullItem<FileGroupFile>(new FileGroupFile { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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