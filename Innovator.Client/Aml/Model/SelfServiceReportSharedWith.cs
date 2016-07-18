using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SelfServiceReportSharedWith </summary>
  public class SelfServiceReportSharedWith : Item
  {
    protected SelfServiceReportSharedWith() { }
    public SelfServiceReportSharedWith(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>access_rights</c> property of the item</summary>
    public IProperty_Text AccessRights()
    {
      return this.Property("access_rights");
    }
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