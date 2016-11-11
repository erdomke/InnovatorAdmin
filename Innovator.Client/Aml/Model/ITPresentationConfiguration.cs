using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ITPresentationConfiguration </summary>
  public class ITPresentationConfiguration : Item, INullRelationship<ItemType>, IRelationship<PresentationConfiguration>
  {
    protected ITPresentationConfiguration() { }
    public ITPresentationConfiguration(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ITPresentationConfiguration() { Innovator.Client.Item.AddNullItem<ITPresentationConfiguration>(new ITPresentationConfiguration { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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