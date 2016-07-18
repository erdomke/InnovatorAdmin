using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type tp_XmlSchemaElement </summary>
  public class tp_XmlSchemaElement : Item
  {
    protected tp_XmlSchemaElement() { }
    public tp_XmlSchemaElement(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>content_generator</c> property of the item</summary>
    public IProperty_Item ContentGenerator()
    {
      return this.Property("content_generator");
    }
    /// <summary>Retrieve the <c>default_classification</c> property of the item</summary>
    public IProperty_Text DefaultClassification()
    {
      return this.Property("default_classification");
    }
    /// <summary>Retrieve the <c>is_content_dynamic</c> property of the item</summary>
    public IProperty_Boolean IsContentDynamic()
    {
      return this.Property("is_content_dynamic");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>renderer</c> property of the item</summary>
    public IProperty_Item Renderer()
    {
      return this.Property("renderer");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}