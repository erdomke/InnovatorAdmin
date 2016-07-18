using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ConversionTaskHandlerError </summary>
  public class ConversionTaskHandlerError : Item
  {
    protected ConversionTaskHandlerError() { }
    public ConversionTaskHandlerError(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>attempt_number</c> property of the item</summary>
    public IProperty_Number AttemptNumber()
    {
      return this.Property("attempt_number");
    }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>error_message</c> property of the item</summary>
    public IProperty_Text ErrorMessage()
    {
      return this.Property("error_message");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}