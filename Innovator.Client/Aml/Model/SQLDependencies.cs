using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SQL Dependencies </summary>
  public class SQLDependencies : Item, INullRelationship<SQL>, IRelationship<SQL>
  {
    protected SQLDependencies() { }
    public SQLDependencies(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SQLDependencies() { Innovator.Client.Item.AddNullItem<SQLDependencies>(new SQLDependencies { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

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