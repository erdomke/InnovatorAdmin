using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Locale </summary>
  public class Locale : Item
  {
    protected Locale() { }
    public Locale(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Locale() { Innovator.Client.Item.AddNullItem<Locale>(new Locale { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>code</c> property of the item</summary>
    public IProperty_Text Code()
    {
      return this.Property("code");
    }
    /// <summary>Retrieve the <c>language</c> property of the item</summary>
    public IProperty_Item<Language> Language()
    {
      return this.Property("language");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
  }
}