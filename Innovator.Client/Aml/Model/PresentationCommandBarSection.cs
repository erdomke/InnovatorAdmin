using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type PresentationCommandBarSection </summary>
  public class PresentationCommandBarSection : Item, INullRelationship<PresentationConfiguration>, IRelationship<CommandBarSection>
  {
    protected PresentationCommandBarSection() { }
    public PresentationCommandBarSection(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static PresentationCommandBarSection() { Innovator.Client.Item.AddNullItem<PresentationCommandBarSection>(new PresentationCommandBarSection { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>item_classification</c> property of the item</summary>
    public IProperty_Text ItemClassification()
    {
      return this.Property("item_classification");
    }
    /// <summary>Retrieve the <c>role</c> property of the item</summary>
    public IProperty_Item<Identity> Role()
    {
      return this.Property("role");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}