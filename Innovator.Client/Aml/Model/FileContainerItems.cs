using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type FileContainerItems </summary>
  public class FileContainerItems : Item, IFileContainerItems
  {
    protected FileContainerItems() { }
    public FileContainerItems(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static FileContainerItems() { Innovator.Client.Item.AddNullItem<FileContainerItems>(new FileContainerItems { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

  }
}