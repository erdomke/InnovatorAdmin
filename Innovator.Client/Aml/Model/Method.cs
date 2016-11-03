using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Method </summary>
  public class Method : Item
  {
    protected Method() { }
    public Method(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Method() { Innovator.Client.Item.AddNullItem<Method>(new Method { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>comments</c> property of the item</summary>
    public IProperty_Text Comments()
    {
      return this.Property("comments");
    }
    /// <summary>Retrieve the <c>core</c> property of the item</summary>
    public IProperty_Boolean Core()
    {
      return this.Property("core");
    }
    /// <summary>Retrieve the <c>effective_date</c> property of the item</summary>
    public IProperty_Date EffectiveDate()
    {
      return this.Property("effective_date");
    }
    /// <summary>Retrieve the <c>execution_allowed_to</c> property of the item</summary>
    public IProperty_Item<Identity> ExecutionAllowedTo()
    {
      return this.Property("execution_allowed_to");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>method_code</c> property of the item</summary>
    public IProperty_Text MethodCode()
    {
      return this.Property("method_code");
    }
    /// <summary>Retrieve the <c>method_location</c> property of the item</summary>
    public IProperty_Text MethodLocation()
    {
      return this.Property("method_location");
    }
    /// <summary>Retrieve the <c>method_type</c> property of the item</summary>
    public IProperty_Text MethodType()
    {
      return this.Property("method_type");
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
    /// <summary>Retrieve the <c>superseded_date</c> property of the item</summary>
    public IProperty_Date SupersededDate()
    {
      return this.Property("superseded_date");
    }
  }
}