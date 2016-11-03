using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Team Identity </summary>
  public class TeamIdentity : Item, INullRelationship<Team>, IRelationship<Identity>
  {
    protected TeamIdentity() { }
    public TeamIdentity(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static TeamIdentity() { Innovator.Client.Item.AddNullItem<TeamIdentity>(new TeamIdentity { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

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
    public IProperty_Item<Identity> TeamRole()
    {
      return this.Property("team_role");
    }
  }
}