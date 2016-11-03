using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type DiscussionTemplate </summary>
  public class DiscussionTemplate : Item, INullRelationship<ItemType>
  {
    protected DiscussionTemplate() { }
    public DiscussionTemplate(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static DiscussionTemplate() { Innovator.Client.Item.AddNullItem<DiscussionTemplate>(new DiscussionTemplate { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>class_path</c> property of the item</summary>
    public IProperty_Text ClassPath()
    {
      return this.Property("class_path");
    }
    /// <summary>Retrieve the <c>file_selection_depth</c> property of the item</summary>
    public IProperty_Number FileSelectionDepth()
    {
      return this.Property("file_selection_depth");
    }
    /// <summary>Retrieve the <c>item_selection_depth</c> property of the item</summary>
    public IProperty_Number ItemSelectionDepth()
    {
      return this.Property("item_selection_depth");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}