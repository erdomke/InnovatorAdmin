using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type cmf_ContentTypeExportSetting </summary>
  public class cmf_ContentTypeExportSetting : Item, Icmf_ContentTypeExportSetting
  {
    protected cmf_ContentTypeExportSetting() { }
    public cmf_ContentTypeExportSetting(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static cmf_ContentTypeExportSetting() { Innovator.Client.Item.AddNullItem<cmf_ContentTypeExportSetting>(new cmf_ContentTypeExportSetting { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

  }
}