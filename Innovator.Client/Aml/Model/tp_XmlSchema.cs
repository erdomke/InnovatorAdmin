using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type tp_XmlSchema </summary>
  public class tp_XmlSchema : Item
  {
    protected tp_XmlSchema() { }
    public tp_XmlSchema(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static tp_XmlSchema() { Innovator.Client.Item.AddNullItem<tp_XmlSchema>(new tp_XmlSchema { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>content</c> property of the item</summary>
    public IProperty_Text Content()
    {
      return this.Property("content");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>target_namespace</c> property of the item</summary>
    public IProperty_Text TargetNamespace()
    {
      return this.Property("target_namespace");
    }
  }
}