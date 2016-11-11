using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Alias </summary>
  public class Alias : Item, INullRelationship<User>, IRelationship<Identity>
  {
    protected Alias() { }
    public Alias(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Alias() { Innovator.Client.Item.AddNullItem<Alias>(new Alias { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>end_date</c> property of the item</summary>
    public IProperty_Date EndDate()
    {
      return this.Property("end_date");
    }
    /// <summary>Retrieve the <c>from_date</c> property of the item</summary>
    public IProperty_Date FromDate()
    {
      return this.Property("from_date");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}