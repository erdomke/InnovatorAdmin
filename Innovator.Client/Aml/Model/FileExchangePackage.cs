using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type FileExchangePackage </summary>
  public class FileExchangePackage : Item
  {
    protected FileExchangePackage() { }
    public FileExchangePackage(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static FileExchangePackage() { Innovator.Client.Item.AddNullItem<FileExchangePackage>(new FileExchangePackage { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>pkg_number</c> property of the item</summary>
    public IProperty_Text PkgNumber()
    {
      return this.Property("pkg_number");
    }
  }
}