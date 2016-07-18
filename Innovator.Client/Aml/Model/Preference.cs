using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Preference </summary>
  public class Preference : Item
  {
    protected Preference() { }
    public Preference(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>identity_id</c> property of the item</summary>
    public IProperty_Item IdentityId()
    {
      return this.Property("identity_id");
    }
  }
}