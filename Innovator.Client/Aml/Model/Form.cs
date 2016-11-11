using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Form </summary>
  public class Form : Item
  {
    protected Form() { }
    public Form(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Form() { Innovator.Client.Item.AddNullItem<Form>(new Form { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>category_form</c> property of the item</summary>
    public IProperty_Boolean CategoryForm()
    {
      return this.Property("category_form");
    }
    /// <summary>Retrieve the <c>core</c> property of the item</summary>
    public IProperty_Boolean Core()
    {
      return this.Property("core");
    }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>height</c> property of the item</summary>
    public IProperty_Number Height()
    {
      return this.Property("height");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>stylesheet</c> property of the item</summary>
    public IProperty_Text Stylesheet()
    {
      return this.Property("stylesheet");
    }
    /// <summary>Retrieve the <c>width</c> property of the item</summary>
    public IProperty_Number Width()
    {
      return this.Property("width");
    }
  }
}