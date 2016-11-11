using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type tp_Block </summary>
  public class tp_Block : Item
  {
    protected tp_Block() { }
    public tp_Block(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static tp_Block() { Innovator.Client.Item.AddNullItem<tp_Block>(new tp_Block { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>condition</c> property of the item</summary>
    public IProperty_Text Condition()
    {
      return this.Property("condition");
    }
    /// <summary>Retrieve the <c>content</c> property of the item</summary>
    public IProperty_Text Content()
    {
      return this.Property("content");
    }
    /// <summary>Retrieve the <c>effective_date</c> property of the item</summary>
    public IProperty_Date EffectiveDate()
    {
      return this.Property("effective_date");
    }
    /// <summary>Retrieve the <c>item_number</c> property of the item</summary>
    public IProperty_Text ItemNumber()
    {
      return this.Property("item_number");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>release_date</c> property of the item</summary>
    public IProperty_Date ReleaseDate()
    {
      return this.Property("release_date");
    }
    /// <summary>Retrieve the <c>root_element_name</c> property of the item</summary>
    public IProperty_Text RootElementName()
    {
      return this.Property("root_element_name");
    }
    /// <summary>Retrieve the <c>root_element_type</c> property of the item</summary>
    public IProperty_Text RootElementType()
    {
      return this.Property("root_element_type");
    }
    /// <summary>Retrieve the <c>superseded_date</c> property of the item</summary>
    public IProperty_Date SupersededDate()
    {
      return this.Property("superseded_date");
    }
    /// <summary>Retrieve the <c>xml_schema</c> property of the item</summary>
    public IProperty_Item<tp_XmlSchema> XmlSchema()
    {
      return this.Property("xml_schema");
    }
  }
}