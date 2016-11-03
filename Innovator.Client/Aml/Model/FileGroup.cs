using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type FileGroup </summary>
  public class FileGroup : Item
  {
    protected FileGroup() { }
    public FileGroup(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static FileGroup() { Innovator.Client.Item.AddNullItem<FileGroup>(new FileGroup { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

  }
}