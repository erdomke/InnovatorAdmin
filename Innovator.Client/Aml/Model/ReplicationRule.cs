using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ReplicationRule </summary>
  public class ReplicationRule : Item, INullRelationship<Vault>, IRelationship<Identity>
  {
    protected ReplicationRule() { }
    public ReplicationRule(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ReplicationRule() { Innovator.Client.Item.AddNullItem<ReplicationRule>(new ReplicationRule { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>do_replicate</c> property of the item</summary>
    public IProperty_Item<Method> DoReplicate()
    {
      return this.Property("do_replicate");
    }
    /// <summary>Retrieve the <c>initiator_type</c> property of the item</summary>
    public IProperty_Text InitiatorType()
    {
      return this.Property("initiator_type");
    }
    /// <summary>Retrieve the <c>is_active</c> property of the item</summary>
    public IProperty_Boolean IsActive()
    {
      return this.Property("is_active");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>max_wait</c> property of the item</summary>
    public IProperty_Text MaxWait()
    {
      return this.Property("max_wait");
    }
    /// <summary>Retrieve the <c>replication_mode</c> property of the item</summary>
    public IProperty_Text ReplicationMode()
    {
      return this.Property("replication_mode");
    }
    /// <summary>Retrieve the <c>replication_time</c> property of the item</summary>
    public IProperty_Text ReplicationTime()
    {
      return this.Property("replication_time");
    }
    /// <summary>Retrieve the <c>replication_type</c> property of the item</summary>
    public IProperty_Text ReplicationType()
    {
      return this.Property("replication_type");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}