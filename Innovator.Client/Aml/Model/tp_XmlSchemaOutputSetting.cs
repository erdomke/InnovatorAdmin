using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type tp_XmlSchemaOutputSetting </summary>
  public class tp_XmlSchemaOutputSetting : Item, INullRelationship<tp_XmlSchema>
  {
    protected tp_XmlSchemaOutputSetting() { }
    public tp_XmlSchemaOutputSetting(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static tp_XmlSchemaOutputSetting() { Innovator.Client.Item.AddNullItem<tp_XmlSchemaOutputSetting>(new tp_XmlSchemaOutputSetting { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>indentation</c> property of the item</summary>
    public IProperty_Boolean Indentation()
    {
      return this.Property("indentation");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>omit_xml_declaration</c> property of the item</summary>
    public IProperty_Boolean OmitXmlDeclaration()
    {
      return this.Property("omit_xml_declaration");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>stylesheet_id</c> property of the item</summary>
    public IProperty_Item<tp_Stylesheet> StylesheetId()
    {
      return this.Property("stylesheet_id");
    }
    /// <summary>Retrieve the <c>target_classification</c> property of the item</summary>
    public IProperty_Text TargetClassification()
    {
      return this.Property("target_classification");
    }
  }
}