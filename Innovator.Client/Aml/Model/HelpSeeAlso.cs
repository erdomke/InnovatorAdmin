using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Help See Also </summary>
  public class HelpSeeAlso : Item, INullRelationship<Help>, IRelationship<Help>
  {
    protected HelpSeeAlso() { }
    public HelpSeeAlso(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static HelpSeeAlso() { Innovator.Client.Item.AddNullItem<HelpSeeAlso>(new HelpSeeAlso { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>seq</c> property of the item</summary>
    public IProperty_Number Seq()
    {
      return this.Property("seq");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}