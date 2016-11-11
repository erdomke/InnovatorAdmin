using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ContentTypeExportRel </summary>
  public class cmf_ContentTypeExportRel : Item, INullRelationship<cmf_ContentType>, IRelationship<cmf_ContentTypeExportSetting>
  {
    protected cmf_ContentTypeExportRel() { }
    public cmf_ContentTypeExportRel(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ContentTypeExportRel() { Innovator.Client.Item.AddNullItem<cmf_ContentTypeExportRel>(new cmf_ContentTypeExportRel { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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