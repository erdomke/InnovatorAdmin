using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Applied Updates </summary>
  public class AppliedUpdates : Item
  {
    protected AppliedUpdates() { }
    public AppliedUpdates(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static AppliedUpdates() { Innovator.Client.Item.AddNullItem<AppliedUpdates>(new AppliedUpdates { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>date_applied</c> property of the item</summary>
    public IProperty_Date DateApplied()
    {
      return this.Property("date_applied");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>up_number</c> property of the item</summary>
    public IProperty_Text UpNumber()
    {
      return this.Property("up_number");
    }
    /// <summary>Retrieve the <c>update_generation</c> property of the item</summary>
    public IProperty_Number UpdateGeneration()
    {
      return this.Property("update_generation");
    }
  }
}