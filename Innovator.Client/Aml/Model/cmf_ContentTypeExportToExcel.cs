using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ContentTypeExportToExcel </summary>
  public class cmf_ContentTypeExportToExcel : Item, Icmf_ContentTypeExportSetting, IFileContainerItems
  {
    protected cmf_ContentTypeExportToExcel() { }
    public cmf_ContentTypeExportToExcel(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ContentTypeExportToExcel() { Innovator.Client.Item.AddNullItem<cmf_ContentTypeExportToExcel>(new cmf_ContentTypeExportToExcel { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>template_file</c> property of the item</summary>
    public IProperty_Item<File> TemplateFile()
    {
      return this.Property("template_file");
    }
  }
}