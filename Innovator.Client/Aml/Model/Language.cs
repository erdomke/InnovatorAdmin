using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Language </summary>
  public class Language : Item
  {
    protected Language() { }
    public Language(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Language() { Innovator.Client.Item.AddNullItem<Language>(new Language { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>code</c> property of the item</summary>
    public IProperty_Text Code()
    {
      return this.Property("code");
    }
    /// <summary>Retrieve the <c>collation</c> property of the item</summary>
    public IProperty_Text Collation()
    {
      return this.Property("collation");
    }
    /// <summary>Retrieve the <c>direction</c> property of the item</summary>
    public IProperty_Text Direction()
    {
      return this.Property("direction");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>suffix</c> property of the item</summary>
    public IProperty_Text Suffix()
    {
      return this.Property("suffix");
    }
  }
}