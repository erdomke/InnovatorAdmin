using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Access </summary>
  public class Access : Item
  {
    protected Access() { }
    public Access(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>can_change_access</c> property of the item</summary>
    public IProperty_Boolean CanChangeAccess()
    {
      return this.Property("can_change_access");
    }
    /// <summary>Retrieve the <c>can_delete</c> property of the item</summary>
    public IProperty_Boolean CanDelete()
    {
      return this.Property("can_delete");
    }
    /// <summary>Retrieve the <c>can_discover</c> property of the item</summary>
    public IProperty_Boolean CanDiscover()
    {
      return this.Property("can_discover");
    }
    /// <summary>Retrieve the <c>can_get</c> property of the item</summary>
    public IProperty_Boolean CanGet()
    {
      return this.Property("can_get");
    }
    /// <summary>Retrieve the <c>can_update</c> property of the item</summary>
    public IProperty_Boolean CanUpdate()
    {
      return this.Property("can_update");
    }
    /// <summary>Retrieve the <c>end_date</c> property of the item</summary>
    public IProperty_Date EndDate()
    {
      return this.Property("end_date");
    }
    /// <summary>Retrieve the <c>from_date</c> property of the item</summary>
    public IProperty_Date FromDate()
    {
      return this.Property("from_date");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>show_permissions_warning</c> property of the item</summary>
    public IProperty_Boolean ShowPermissionsWarning()
    {
      return this.Property("show_permissions_warning");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
    /// <summary>Retrieve the <c>zone</c> property of the item</summary>
    public IProperty_Text Zone()
    {
      return this.Property("zone");
    }
  }
}