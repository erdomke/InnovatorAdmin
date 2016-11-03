using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type DatabaseUpgrade </summary>
  public class DatabaseUpgrade : Item
  {
    protected DatabaseUpgrade() { }
    public DatabaseUpgrade(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static DatabaseUpgrade() { Innovator.Client.Item.AddNullItem<DatabaseUpgrade>(new DatabaseUpgrade { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>applied_on</c> property of the item</summary>
    public IProperty_Date AppliedOn()
    {
      return this.Property("applied_on");
    }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>is_latest</c> property of the item</summary>
    public IProperty_Boolean IsLatest()
    {
      return this.Property("is_latest");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>os_user</c> property of the item</summary>
    public IProperty_Text OsUser()
    {
      return this.Property("os_user");
    }
    /// <summary>Retrieve the <c>target_release</c> property of the item</summary>
    public IProperty_Text TargetRelease()
    {
      return this.Property("target_release");
    }
    /// <summary>Retrieve the <c>type</c> property of the item</summary>
    public IProperty_Text Type()
    {
      return this.Property("type");
    }
    /// <summary>Retrieve the <c>upgrade_status</c> property of the item</summary>
    public IProperty_Text UpgradeStatus()
    {
      return this.Property("upgrade_status");
    }
  }
}