using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Preference </summary>
  public class Preference : Item
  {
    protected Preference() { }
    public Preference(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static Preference() { Innovator.Client.Item.AddNullItem<Preference>(new Preference { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>identity_id</c> property of the item</summary>
    public IProperty_Item<Identity> IdentityId()
    {
      return this.Property("identity_id");
    }
  }
}