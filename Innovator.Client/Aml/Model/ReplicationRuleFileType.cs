using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ReplicationRuleFileType </summary>
  public class ReplicationRuleFileType : Item, INullRelationship<ReplicationRule>, IRelationship<FileType>
  {
    protected ReplicationRuleFileType() { }
    public ReplicationRuleFileType(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ReplicationRuleFileType() { Innovator.Client.Item.AddNullItem<ReplicationRuleFileType>(new ReplicationRuleFileType { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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