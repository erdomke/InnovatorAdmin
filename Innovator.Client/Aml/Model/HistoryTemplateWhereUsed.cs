using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type History Template Where Used </summary>
  public class HistoryTemplateWhereUsed : Item, INullRelationship<HistoryTemplate>
  {
    protected HistoryTemplateWhereUsed() { }
    public HistoryTemplateWhereUsed(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static HistoryTemplateWhereUsed() { Innovator.Client.Item.AddNullItem<HistoryTemplateWhereUsed>(new HistoryTemplateWhereUsed { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

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