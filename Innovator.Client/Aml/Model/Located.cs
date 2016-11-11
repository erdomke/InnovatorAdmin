using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Located </summary>
  public class Located : Item, INullRelationship<File>, IRelationship<Vault>
  {
    protected Located() { }
    public Located(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Located() { Innovator.Client.Item.AddNullItem<Located>(new Located { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>file_version</c> property of the item</summary>
    public IProperty_Number FileVersion()
    {
      return this.Property("file_version");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}