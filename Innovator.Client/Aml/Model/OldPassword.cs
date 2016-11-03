using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Old Password </summary>
  public class OldPassword : Item, INullRelationship<User>
  {
    protected OldPassword() { }
    public OldPassword(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static OldPassword() { Innovator.Client.Item.AddNullItem<OldPassword>(new OldPassword { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>old_pwd</c> property of the item</summary>
    public IProperty_Text OldPwd()
    {
      return this.Property("old_pwd");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}