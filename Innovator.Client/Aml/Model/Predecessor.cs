using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Predecessor </summary>
  public class Predecessor : Item
  {
    protected Predecessor() { }
    public Predecessor(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>lead_lag</c> property of the item</summary>
    public IProperty_Number LeadLag()
    {
      return this.Property("lead_lag");
    }
    /// <summary>Retrieve the <c>precedence_type</c> property of the item</summary>
    public IProperty_Text PrecedenceType()
    {
      return this.Property("precedence_type");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}