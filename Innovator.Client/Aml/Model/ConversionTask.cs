using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ConversionTask </summary>
  public class ConversionTask : Item
  {
    protected ConversionTask() { }
    public ConversionTask(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ConversionTask() { Innovator.Client.Item.AddNullItem<ConversionTask>(new ConversionTask { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>error</c> property of the item</summary>
    public IProperty_Text Error()
    {
      return this.Property("error");
    }
    /// <summary>Retrieve the <c>file_id</c> property of the item</summary>
    public IProperty_Text FileId()
    {
      return this.Property("file_id");
    }
    /// <summary>Retrieve the <c>file_type</c> property of the item</summary>
    public IProperty_Item<FileType> FileType()
    {
      return this.Property("file_type");
    }
    /// <summary>Retrieve the <c>finished_on</c> property of the item</summary>
    public IProperty_Date FinishedOn()
    {
      return this.Property("finished_on");
    }
    /// <summary>Retrieve the <c>rule_id</c> property of the item</summary>
    public IProperty_Item<ConversionRule> RuleId()
    {
      return this.Property("rule_id");
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