using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type DatabaseUpgradeLogFile </summary>
  public class DatabaseUpgradeLogFile : Item, IFileContainerItems, INullRelationship<DatabaseUpgrade>, IRelationship<File>
  {
    protected DatabaseUpgradeLogFile() { }
    public DatabaseUpgradeLogFile(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static DatabaseUpgradeLogFile() { Innovator.Client.Item.AddNullItem<DatabaseUpgradeLogFile>(new DatabaseUpgradeLogFile { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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