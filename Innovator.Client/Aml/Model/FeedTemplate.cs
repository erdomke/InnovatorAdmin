using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type FeedTemplate </summary>
  public class FeedTemplate : Item, INullRelationship<DiscussionTemplate>
  {
    protected FeedTemplate() { }
    public FeedTemplate(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static FeedTemplate() { Innovator.Client.Item.AddNullItem<FeedTemplate>(new FeedTemplate { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>feed_type</c> property of the item</summary>
    public IProperty_Text FeedType()
    {
      return this.Property("feed_type");
    }
    /// <summary>Retrieve the <c>history_events</c> property of the item</summary>
    public IProperty_Text HistoryEvents()
    {
      return this.Property("history_events");
    }
    /// <summary>Retrieve the <c>polysource_type_name</c> property of the item</summary>
    public IProperty_Text PolysourceTypeName()
    {
      return this.Property("polysource_type_name");
    }
    /// <summary>Retrieve the <c>reference</c> property of the item</summary>
    public IProperty_Text Reference()
    {
      return this.Property("reference");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}