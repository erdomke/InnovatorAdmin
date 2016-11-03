using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Forum </summary>
  public class Forum : Item, ISSVCItems
  {
    protected Forum() { }
    public Forum(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Forum() { Innovator.Client.Item.AddNullItem<Forum>(new Forum { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>forum_type</c> property of the item</summary>
    public IProperty_Text ForumType()
    {
      return this.Property("forum_type");
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
  }
}