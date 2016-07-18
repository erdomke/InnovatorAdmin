using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type FileExchangeTxn </summary>
  public class FileExchangeTxn : Item
  {
    protected FileExchangeTxn() { }
    public FileExchangeTxn(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>comments</c> property of the item</summary>
    public IProperty_Text Comments()
    {
      return this.Property("comments");
    }
    /// <summary>Retrieve the <c>data</c> property of the item</summary>
    public IProperty_Text Data()
    {
      return this.Property("data");
    }
    /// <summary>Retrieve the <c>error</c> property of the item</summary>
    public IProperty_Text Error()
    {
      return this.Property("error");
    }
    /// <summary>Retrieve the <c>package</c> property of the item</summary>
    public IProperty_Item Package()
    {
      return this.Property("package");
    }
    /// <summary>Retrieve the <c>service_provider</c> property of the item</summary>
    public IProperty_Text ServiceProvider()
    {
      return this.Property("service_provider");
    }
    /// <summary>Retrieve the <c>transfer_attempts</c> property of the item</summary>
    public IProperty_Number TransferAttempts()
    {
      return this.Property("transfer_attempts");
    }
    /// <summary>Retrieve the <c>txn_number</c> property of the item</summary>
    public IProperty_Text TxnNumber()
    {
      return this.Property("txn_number");
    }
    /// <summary>Retrieve the <c>txn_status</c> property of the item</summary>
    public IProperty_Text TxnStatus()
    {
      return this.Property("txn_status");
    }
    /// <summary>Retrieve the <c>txn_type</c> property of the item</summary>
    public IProperty_Text TxnType()
    {
      return this.Property("txn_type");
    }
  }
}