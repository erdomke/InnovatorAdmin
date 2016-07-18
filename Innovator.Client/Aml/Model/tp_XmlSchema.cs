using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type tp_XmlSchema </summary>
  public class tp_XmlSchema : Item
  {
    protected tp_XmlSchema() { }
    public tp_XmlSchema(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
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