using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Revision </summary>
  public class Revision : Item
  {
    protected Revision() { }
    public Revision(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Revision() { Innovator.Client.Item.AddNullItem<Revision>(new Revision { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

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
    /// <summary>Retrieve the <c>revision</c> property of the item</summary>
    public IProperty_Text RevisionProp()
    {
      return this.Property("revision");
    }
  }
}