using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ConversionTaskEventHandler </summary>
  public class ConversionTaskEventHandler : Item, INullRelationship<ConversionTask>, IRelationship<Method>
  {
    protected ConversionTaskEventHandler() { }
    public ConversionTaskEventHandler(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ConversionTaskEventHandler() { Innovator.Client.Item.AddNullItem<ConversionTaskEventHandler>(new ConversionTaskEventHandler { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>event_type</c> property of the item</summary>
    public IProperty_Text EventType()
    {
      return this.Property("event_type");
    }
    /// <summary>Retrieve the <c>execution_attempt</c> property of the item</summary>
    public IProperty_Number ExecutionAttempt()
    {
      return this.Property("execution_attempt");
    }
    /// <summary>Retrieve the <c>finished_on</c> property of the item</summary>
    public IProperty_Date FinishedOn()
    {
      return this.Property("finished_on");
    }
    /// <summary>Retrieve the <c>lock_dependencies</c> property of the item</summary>
    public IProperty_Boolean LockDependencies()
    {
      return this.Property("lock_dependencies");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>started_on</c> property of the item</summary>
    public IProperty_Date StartedOn()
    {
      return this.Property("started_on");
    }
    /// <summary>Retrieve the <c>status</c> property of the item</summary>
    public IProperty_Text Status()
    {
      return this.Property("status");
    }
    /// <summary>Retrieve the <c>user_data</c> property of the item</summary>
    public IProperty_Text UserData()
    {
      return this.Property("user_data");
    }
  }
}