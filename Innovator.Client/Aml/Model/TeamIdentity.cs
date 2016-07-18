using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Team Identity </summary>
  public class TeamIdentity : Item
  {
    protected TeamIdentity() { }
    public TeamIdentity(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>team_role</c> property of the item</summary>
    public IProperty_Item TeamRole()
    {
      return this.Property("team_role");
    }
  }
}