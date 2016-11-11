using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SelfServiceReportHelp </summary>
  public class SelfServiceReportHelp : Item
  {
    protected SelfServiceReportHelp() { }
    public SelfServiceReportHelp(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SelfServiceReportHelp() { Innovator.Client.Item.AddNullItem<SelfServiceReportHelp>(new SelfServiceReportHelp { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>context_key</c> property of the item</summary>
    public IProperty_Text ContextKey()
    {
      return this.Property("context_key");
    }
    /// <summary>Retrieve the <c>rich_help_text</c> property of the item</summary>
    public IProperty_Text RichHelpText()
    {
      return this.Property("rich_help_text");
    }
  }
}