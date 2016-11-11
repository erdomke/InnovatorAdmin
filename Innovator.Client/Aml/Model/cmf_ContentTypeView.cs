using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ContentTypeView </summary>
  public class cmf_ContentTypeView : Item, INullRelationship<cmf_ContentType>, IRelationship<cmf_BaseView>
  {
    protected cmf_ContentTypeView() { }
    public cmf_ContentTypeView(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ContentTypeView() { Innovator.Client.Item.AddNullItem<cmf_ContentTypeView>(new cmf_ContentTypeView { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}