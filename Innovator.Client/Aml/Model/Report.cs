using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Report </summary>
  public class Report : Item
  {
    protected Report() { }
    public Report(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>location</c> property of the item</summary>
    public IProperty_Text Location()
    {
      return this.Property("location");
    }
    /// <summary>Retrieve the <c>method</c> property of the item</summary>
    public IProperty_Item Method()
    {
      return this.Property("method");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>report_query</c> property of the item</summary>
    public IProperty_Text ReportQuery()
    {
      return this.Property("report_query");
    }
    /// <summary>Retrieve the <c>target</c> property of the item</summary>
    public IProperty_Text Target()
    {
      return this.Property("target");
    }
    /// <summary>Retrieve the <c>type</c> property of the item</summary>
    public IProperty_Text Type()
    {
      return this.Property("type");
    }
    /// <summary>Retrieve the <c>xsl_stylesheet</c> property of the item</summary>
    public IProperty_Text XslStylesheet()
    {
      return this.Property("xsl_stylesheet");
    }
  }
}