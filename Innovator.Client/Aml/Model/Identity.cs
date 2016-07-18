using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Identity </summary>
  public class Identity : Item
  {
    protected Identity() { }
    public Identity(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>is_alias</c> property of the item</summary>
    public IProperty_Boolean IsAlias()
    {
      return this.Property("is_alias");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>maximum_pwd_age</c> property of the item</summary>
    public IProperty_Number MaximumPwdAge()
    {
      return this.Property("maximum_pwd_age");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>pwd_history_length</c> property of the item</summary>
    public IProperty_Number PwdHistoryLength()
    {
      return this.Property("pwd_history_length");
    }
  }
}