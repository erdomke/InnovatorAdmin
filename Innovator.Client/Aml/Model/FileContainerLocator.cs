using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type FileContainerLocator </summary>
  public class FileContainerLocator : Item
  {
    protected FileContainerLocator() { }
    public FileContainerLocator(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>container_id</c> property of the item</summary>
    public IProperty_Item ContainerId()
    {
      return this.Property("container_id");
    }
    /// <summary>Retrieve the <c>container_property_name</c> property of the item</summary>
    public IProperty_Text ContainerPropertyName()
    {
      return this.Property("container_property_name");
    }
    /// <summary>Retrieve the <c>container_type_id</c> property of the item</summary>
    public IProperty_Item ContainerTypeId()
    {
      return this.Property("container_type_id");
    }
    /// <summary>Retrieve the <c>file_id</c> property of the item</summary>
    public IProperty_Item FileId()
    {
      return this.Property("file_id");
    }
  }
}