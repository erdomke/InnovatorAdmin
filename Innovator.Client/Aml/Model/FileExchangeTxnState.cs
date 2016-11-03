using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type FileExchangeTxnState </summary>
  public class FileExchangeTxnState : Item, INullRelationship<FileExchangeTxn>
  {
    protected FileExchangeTxnState() { }
    public FileExchangeTxnState(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static FileExchangeTxnState() { Innovator.Client.Item.AddNullItem<FileExchangeTxnState>(new FileExchangeTxnState { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>error</c> property of the item</summary>
    public IProperty_Text Error()
    {
      return this.Property("error");
    }
    /// <summary>Retrieve the <c>file_id</c> property of the item</summary>
    public IProperty_Text FileId()
    {
      return this.Property("file_id");
    }
    /// <summary>Retrieve the <c>file_name</c> property of the item</summary>
    public IProperty_Text FileName()
    {
      return this.Property("file_name");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>status</c> property of the item</summary>
    public IProperty_Text Status()
    {
      return this.Property("status");
    }
  }
}