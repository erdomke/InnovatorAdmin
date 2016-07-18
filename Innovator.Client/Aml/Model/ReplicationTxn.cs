using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ReplicationTxn </summary>
  public class ReplicationTxn : Item
  {
    protected ReplicationTxn() { }
    public ReplicationTxn(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>end_time</c> property of the item</summary>
    public IProperty_Date EndTime()
    {
      return this.Property("end_time");
    }
    /// <summary>Retrieve the <c>error_msg</c> property of the item</summary>
    public IProperty_Text ErrorMsg()
    {
      return this.Property("error_msg");
    }
    /// <summary>Retrieve the <c>execution_attempt</c> property of the item</summary>
    public IProperty_Number ExecutionAttempt()
    {
      return this.Property("execution_attempt");
    }
    /// <summary>Retrieve the <c>file_id</c> property of the item</summary>
    public IProperty_Text FileId()
    {
      return this.Property("file_id");
    }
    /// <summary>Retrieve the <c>from_vault</c> property of the item</summary>
    public IProperty_Item FromVault()
    {
      return this.Property("from_vault");
    }
    /// <summary>Retrieve the <c>not_before</c> property of the item</summary>
    public IProperty_Date NotBefore()
    {
      return this.Property("not_before");
    }
    /// <summary>Retrieve the <c>replication_rule</c> property of the item</summary>
    public IProperty_Item ReplicationRule()
    {
      return this.Property("replication_rule");
    }
    /// <summary>Retrieve the <c>replication_status</c> property of the item</summary>
    public IProperty_Text ReplicationStatus()
    {
      return this.Property("replication_status");
    }
    /// <summary>Retrieve the <c>start_time</c> property of the item</summary>
    public IProperty_Date StartTime()
    {
      return this.Property("start_time");
    }
    /// <summary>Retrieve the <c>to_vault</c> property of the item</summary>
    public IProperty_Item ToVault()
    {
      return this.Property("to_vault");
    }
    /// <summary>Retrieve the <c>user_id</c> property of the item</summary>
    public IProperty_Item UserId()
    {
      return this.Property("user_id");
    }
  }
}