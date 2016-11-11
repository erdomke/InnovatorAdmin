using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type tp_ImageReference </summary>
  public class tp_ImageReference : Item, INullRelationship<tp_Block>, IRelationship<tp_Image>
  {
    protected tp_ImageReference() { }
    public tp_ImageReference(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static tp_ImageReference() { Innovator.Client.Item.AddNullItem<tp_ImageReference>(new tp_ImageReference { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>ignore_all_versions</c> property of the item</summary>
    public IProperty_Boolean IgnoreAllVersions()
    {
      return this.Property("ignore_all_versions");
    }
    /// <summary>Retrieve the <c>ignored_version_id</c> property of the item</summary>
    public IProperty_Text IgnoredVersionId()
    {
      return this.Property("ignored_version_id");
    }
    /// <summary>Retrieve the <c>reference_id</c> property of the item</summary>
    public IProperty_Text ReferenceId()
    {
      return this.Property("reference_id");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}