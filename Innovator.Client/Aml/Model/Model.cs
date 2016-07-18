using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Model </summary>
  public class Model : Item
  {
    protected Model() { }
    public Model(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>item_number</c> property of the item</summary>
    public IProperty_Text ItemNumber()
    {
      return this.Property("item_number");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>part_number</c> property of the item</summary>
    public IProperty_Item PartNumber()
    {
      return this.Property("part_number");
    }
    /// <summary>Retrieve the <c>release_number</c> property of the item</summary>
    public IProperty_Text ReleaseNumber()
    {
      return this.Property("release_number");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>version_number</c> property of the item</summary>
    public IProperty_Text VersionNumber()
    {
      return this.Property("version_number");
    }
  }
}