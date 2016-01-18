using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public static class Core
  {
    #region "Property Attributes"
    public static IAttribute Condition(this IProperty item)
    {
      return item.Attribute("condition");
    }
    public static IAttribute IsNull(this IProperty item)
    {
      return item.Attribute("is_null");
    }
    public static IAttribute KeyedName(this IProperty item)
    {
      return item.Attribute("keyed_name");
    }
    public static IAttribute Type(this IProperty item)
    {
      return item.Attribute("type");
    }
    #endregion
    
    #region "Item Attributes"
    public static IAttribute Action(this IItem item)
    {
      return item.Attribute("action");
    }
    public static IAttribute DoGetItem(this IItem item)
    {
      return item.Attribute("doGetItem");
    }
    public static IAttribute IdList(this IItem item)
    {
      return item.Attribute("idlist");
    }
    public static IAttribute MaxRecords(this IItem item)
    {
      return item.Attribute("maxRecords");
    }
    public static IAttribute OrderBy(this IItem item)
    {
      return item.Attribute("orderBy");
    }
    public static IAttribute Page(this IItem item)
    {
      return item.Attribute("page");
    }
    public static IAttribute PageSize(this IItem item)
    {
      return item.Attribute("pagesize");
    }
    public static IAttribute QueryDate(this IItem item)
    {
      return item.Attribute("queryDate");
    }
    public static IAttribute QueryType(this IItem item)
    {
      return item.Attribute("queryType");
    }
    public static IAttribute RelatedExpand(this IItem item)
    {
      return item.Attribute("related_expand");
    }
    public static IAttribute Select(this IItem item)
    {
      return item.Attribute("select");
    }
    public static IAttribute ServerEvents(this IItem item)
    {
      return item.Attribute("serverEvents");
    }
    public static IAttribute Type(this IItem item)
    {
      return item.Attribute("type");
    }
    public static IAttribute TypeId(this IItem item)
    {
      return item.Attribute("typeID");
    }
    public static IAttribute Where(this IItem item)
    {
      return item.Attribute("where");
    }
    #endregion
    
    #region "Item Properties"
    public static IProperty Classification(this IItem parent)
    {
      return parent.Property("classification");
    }
    public static IProperty ConfigId(this IItem parent)
    {
      return parent.Property("config_id");
    }
    public static IProperty CreatedById(this IItem parent)
    {
      return parent.Property("created_by_id");
    }
    public static IProperty CreatedOn(this IItem parent)
    {
      return parent.Property("created_on");
    }
    public static IProperty Css(this IItem parent)
    {
      return parent.Property("css");
    }
    public static IProperty CurrentState(this IItem parent)
    {
      return parent.Property("current_state");
    }
    public static IProperty Generation(this IItem parent)
    {
      return parent.Property("generation");
    }
    public static IProperty IsCurrent(this IItem parent)
    {
      return parent.Property("is_current");
    }
    public static IProperty IsReleased(this IItem parent)
    {
      return parent.Property("is_released");
    }
    public static IProperty KeyedName(this IItem parent)
    {
      return parent.Property("keyed_name");
    }
    public static IProperty LockedById(this IItem parent)
    {
      return parent.Property("locked_by_id");
    }
    public static IProperty MajorRev(this IItem parent)
    {
      return parent.Property("major_rev");
    }
    public static IProperty ManagedById(this IItem parent)
    {
      return parent.Property("managed_by_id");
    }
    public static IProperty MinorRev(this IItem parent)
    {
      return parent.Property("minor_rev");
    }
    public static IProperty ModifiedById(this IItem parent)
    {
      return parent.Property("modified_by_id");
    }
    public static IProperty ModifiedOn(this IItem parent)
    {
      return parent.Property("modified_on");
    }
    public static IProperty NewVersion(this IItem parent)
    {
      return parent.Property("new_version");
    }
    public static IProperty NotLockable(this IItem parent)
    {
      return parent.Property("not_lockable");
    }
    public static IProperty OwnedById(this IItem parent)
    {
      return parent.Property("owned_by_id");
    }
    public static IProperty PermissionId(this IItem parent)
    {
      return parent.Property("permission_id");
    }
    public static IProperty RelatedId(this IItem parent)
    {
      return parent.Property("related_id");
    }
    public static IItem RelatedItem(this IItem parent)
    {
      return parent.Property("related_id").AsItem();
    }
    public static IProperty State(this IItem parent)
    {
      return parent.Property("state");
    }
    public static IProperty SourceId(this IItem parent)
    {
      return parent.Property("source_id");
    }
    public static IItem SourceItem(this IItem parent)
    {
      var par = parent.Parent; // Relationships
      if (par != null && par is Relationships)
      {
        return par.Parent as IItem;
      }
      return parent.Property("source_id").AsItem();
    }
    #endregion

    #region "Read Only Property Attributes"
    public static IReadOnlyAttribute Condition(this IReadOnlyProperty item)
    {
      return item.Attribute("condition");
    }
    public static IReadOnlyAttribute IsNull(this IReadOnlyProperty item)
    {
      return item.Attribute("is_null");
    }
    public static IReadOnlyAttribute KeyedName(this IReadOnlyProperty item)
    {
      return item.Attribute("keyed_name");
    }
    public static IReadOnlyAttribute Type(this IReadOnlyProperty item)
    {
      return item.Attribute("type");
    }
    #endregion

    #region "Read Only Item Attributes"
    public static IReadOnlyAttribute Action(this IReadOnlyItem item)
    {
      return item.Attribute("action");
    }
    public static IReadOnlyAttribute DoGetItem(this IReadOnlyItem item)
    {
      return item.Attribute("doGetItem");
    }
    public static IReadOnlyAttribute IdList(this IReadOnlyItem item)
    {
      return item.Attribute("idlist");
    }
    public static IReadOnlyAttribute MaxRecords(this IReadOnlyItem item)
    {
      return item.Attribute("maxRecords");
    }
    public static IReadOnlyAttribute OrderBy(this IReadOnlyItem item)
    {
      return item.Attribute("orderBy");
    }
    public static IReadOnlyAttribute Page(this IReadOnlyItem item)
    {
      return item.Attribute("page");
    }
    public static IReadOnlyAttribute PageSize(this IReadOnlyItem item)
    {
      return item.Attribute("pagesize");
    }
    public static IReadOnlyAttribute QueryDate(this IReadOnlyItem item)
    {
      return item.Attribute("queryDate");
    }
    public static IReadOnlyAttribute QueryType(this IReadOnlyItem item)
    {
      return item.Attribute("queryType");
    }
    public static IReadOnlyAttribute RelatedExpand(this IReadOnlyItem item)
    {
      return item.Attribute("related_expand");
    }
    public static IReadOnlyAttribute Select(this IReadOnlyItem item)
    {
      return item.Attribute("select");
    }
    public static IReadOnlyAttribute ServerEvents(this IReadOnlyItem item)
    {
      return item.Attribute("serverEvents");
    }
    public static IReadOnlyAttribute Type(this IReadOnlyItem item)
    {
      return item.Attribute("type");
    }
    public static IReadOnlyAttribute TypeId(this IReadOnlyItem item)
    {
      return item.Attribute("typeID");
    }
    public static IReadOnlyAttribute Where(this IReadOnlyItem item)
    {
      return item.Attribute("where");
    }
    #endregion

    #region "Read Only Item Properties"
    public static IReadOnlyProperty Classification(this IReadOnlyItem parent)
    {
      return parent.Property("classification");
    }
    public static IReadOnlyProperty ConfigId(this IReadOnlyItem parent)
    {
      return parent.Property("config_id");
    }
    public static IReadOnlyProperty CreatedById(this IReadOnlyItem parent)
    {
      return parent.Property("created_by_id");
    }
    public static IReadOnlyProperty CreatedOn(this IReadOnlyItem parent)
    {
      return parent.Property("created_on");
    }
    public static IReadOnlyProperty Css(this IReadOnlyItem parent)
    {
      return parent.Property("css");
    }
    public static IReadOnlyProperty CurrentState(this IReadOnlyItem parent)
    {
      return parent.Property("current_state");
    }
    public static IReadOnlyProperty Generation(this IReadOnlyItem parent)
    {
      return parent.Property("generation");
    }
    public static IReadOnlyProperty IsCurrent(this IReadOnlyItem parent)
    {
      return parent.Property("is_current");
    }
    public static IReadOnlyProperty IsReleased(this IReadOnlyItem parent)
    {
      return parent.Property("is_released");
    }
    public static IReadOnlyProperty KeyedName(this IReadOnlyItem parent)
    {
      return parent.Property("keyed_name");
    }
    public static IReadOnlyProperty LockedById(this IReadOnlyItem parent)
    {
      return parent.Property("locked_by_id");
    }
    public static IReadOnlyProperty MajorRev(this IReadOnlyItem parent)
    {
      return parent.Property("major_rev");
    }
    public static IReadOnlyProperty ManagedById(this IReadOnlyItem parent)
    {
      return parent.Property("managed_by_id");
    }
    public static IReadOnlyProperty MinorRev(this IReadOnlyItem parent)
    {
      return parent.Property("minor_rev");
    }
    public static IReadOnlyProperty ModifiedById(this IReadOnlyItem parent)
    {
      return parent.Property("modified_by_id");
    }
    public static IReadOnlyProperty ModifiedOn(this IReadOnlyItem parent)
    {
      return parent.Property("modified_on");
    }
    public static IReadOnlyProperty NewVersion(this IReadOnlyItem parent)
    {
      return parent.Property("new_version");
    }
    public static IReadOnlyProperty NotLockable(this IReadOnlyItem parent)
    {
      return parent.Property("not_lockable");
    }
    public static IReadOnlyProperty OwnedById(this IReadOnlyItem parent)
    {
      return parent.Property("owned_by_id");
    }
    public static IReadOnlyProperty PermissionId(this IReadOnlyItem parent)
    {
      return parent.Property("permission_id");
    }
    public static IReadOnlyProperty RelatedId(this IReadOnlyItem parent)
    {
      return parent.Property("related_id");
    }
    public static IReadOnlyItem RelatedItem(this IReadOnlyItem parent)
    {
      return parent.Property("related_id").AsItem();
    }
    public static IReadOnlyProperty State(this IReadOnlyItem parent)
    {
      return parent.Property("state");
    }
    public static IReadOnlyProperty SourceId(this IReadOnlyItem parent)
    {
      return parent.Property("source_id");
    }
    public static IReadOnlyItem SourceItem(this IReadOnlyItem parent)
    {
      var par = parent.Parent; // Relationships
      if (par != null && par is Relationships)
      {
        return par.Parent as IReadOnlyItem;
      }
      return parent.Property("source_id").AsItem();
    }
    #endregion
  }
}
