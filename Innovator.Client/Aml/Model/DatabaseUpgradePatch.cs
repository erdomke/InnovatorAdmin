using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type DatabaseUpgradePatch </summary>
  public class DatabaseUpgradePatch : Item, INullRelationship<DatabaseUpgrade>, IRelationship<AppliedUpdates>
  {
    protected DatabaseUpgradePatch() { }
    public DatabaseUpgradePatch(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static DatabaseUpgradePatch() { Innovator.Client.Item.AddNullItem<DatabaseUpgradePatch>(new DatabaseUpgradePatch { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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