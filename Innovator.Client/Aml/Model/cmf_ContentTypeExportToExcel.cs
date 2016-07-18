using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ContentTypeExportToExcel </summary>
  public class cmf_ContentTypeExportToExcel : Item
  {
    protected cmf_ContentTypeExportToExcel() { }
    public cmf_ContentTypeExportToExcel(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>template_file</c> property of the item</summary>
    public IProperty_Item TemplateFile()
    {
      return this.Property("template_file");
    }
  }
}