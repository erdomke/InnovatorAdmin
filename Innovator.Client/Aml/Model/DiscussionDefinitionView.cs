using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type DiscussionDefinitionView </summary>
  public class DiscussionDefinitionView : Item, INullRelationship<DiscussionDefinition>, IRelationship<SSVCPresentationConfiguration>
  {
    protected DiscussionDefinitionView() { }
    public DiscussionDefinitionView(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static DiscussionDefinitionView() { Innovator.Client.Item.AddNullItem<DiscussionDefinitionView>(new DiscussionDefinitionView { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>client</c> property of the item</summary>
    public IProperty_Text Client()
    {
      return this.Property("client");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}