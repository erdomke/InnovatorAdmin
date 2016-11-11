using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Permission </summary>
  public class Permission : Item
  {
    protected Permission() { }
    public Permission(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Permission() { Innovator.Client.Item.AddNullItem<Permission>(new Permission { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>is_private</c> property of the item</summary>
    public IProperty_Boolean IsPrivate()
    {
      return this.Property("is_private");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
  }
}