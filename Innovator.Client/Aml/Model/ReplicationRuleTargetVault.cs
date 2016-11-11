using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ReplicationRuleTargetVault </summary>
  public class ReplicationRuleTargetVault : Item, INullRelationship<ReplicationRule>, IRelationship<Vault>
  {
    protected ReplicationRuleTargetVault() { }
    public ReplicationRuleTargetVault(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ReplicationRuleTargetVault() { Innovator.Client.Item.AddNullItem<ReplicationRuleTargetVault>(new ReplicationRuleTargetVault { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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