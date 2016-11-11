using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type tp_Image </summary>
  public class tp_Image : Item
  {
    protected tp_Image() { }
    public tp_Image(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static tp_Image() { Innovator.Client.Item.AddNullItem<tp_Image>(new tp_Image { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

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
    /// <summary>Retrieve the <c>src</c> property of the item</summary>
    public IProperty_Text Src()
    {
      return this.Property("src");
    }
    /// <summary>Retrieve the <c>superseded_date</c> property of the item</summary>
    public IProperty_Date SupersededDate()
    {
      return this.Property("superseded_date");
    }
  }
}