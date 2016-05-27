using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Extension methods for retrieving core AML properties and attributes
  /// </summary>
  public static class Core
  {
    #region "Property Attributes"
    /// <summary>Retrieve the <c>condition</c> attribute of the property</summary>
    public static IAttribute Condition(this IProperty item)
    {
      return item.Attribute("condition");
    }
    /// <summary>Retrieve the <c>is_null</c> attribute of the property</summary>
    public static IAttribute IsNull(this IProperty item)
    {
      return item.Attribute("is_null");
    }
    /// <summary>Retrieve the <c>keyed_name</c> attribute of the property</summary>
    public static IAttribute KeyedName(this IProperty item)
    {
      return item.Attribute("keyed_name");
    }
    /// <summary>Retrieve the <c>type</c> attribute of the property</summary>
    public static IAttribute Type(this IProperty item)
    {
      return item.Attribute("type");
    }
    #endregion

    #region "Item Attributes"
    /// <summary>Retrieve the <c>action</c> attribute of the item</summary>
    public static IAttribute Action(this IItem item)
    {
      return item.Attribute("action");
    }
    /// <summary>Retrieve the <c>doGetItem</c> attribute of the item</summary>
    public static IAttribute DoGetItem(this IItem item)
    {
      return item.Attribute("doGetItem");
    }
    /// <summary>Retrieve the <c>idlist</c> attribute of the item</summary>
    public static IAttribute IdList(this IItem item)
    {
      return item.Attribute("idlist");
    }
    /// <summary>Retrieve the <c>maxRecords</c> attribute of the item</summary>
    public static IAttribute MaxRecords(this IItem item)
    {
      return item.Attribute("maxRecords");
    }
    /// <summary>Retrieve the <c>orderBy</c> attribute of the item</summary>
    public static IAttribute OrderBy(this IItem item)
    {
      return item.Attribute("orderBy");
    }
    /// <summary>Retrieve the <c>page</c> attribute of the item</summary>
    public static IAttribute Page(this IItem item)
    {
      return item.Attribute("page");
    }
    /// <summary>Retrieve the <c>pagesize</c> attribute of the item</summary>
    public static IAttribute PageSize(this IItem item)
    {
      return item.Attribute("pagesize");
    }
    /// <summary>Retrieve the <c>queryDate</c> attribute of the item</summary>
    public static IAttribute QueryDate(this IItem item)
    {
      return item.Attribute("queryDate");
    }
    /// <summary>Retrieve the <c>queryType</c> attribute of the item</summary>
    public static IAttribute QueryType(this IItem item)
    {
      return item.Attribute("queryType");
    }
    /// <summary>Retrieve the <c>related_expand</c> attribute of the item</summary>
    public static IAttribute RelatedExpand(this IItem item)
    {
      return item.Attribute("related_expand");
    }
    /// <summary>Retrieve the <c>select</c> attribute of the item</summary>
    public static IAttribute Select(this IItem item)
    {
      return item.Attribute("select");
    }
    /// <summary>Retrieve the <c>serverEvents</c> attribute of the item</summary>
    public static IAttribute ServerEvents(this IItem item)
    {
      return item.Attribute("serverEvents");
    }
    /// <summary>Retrieve the <c>type</c> attribute of the item</summary>
    public static IAttribute Type(this IItem item)
    {
      return item.Attribute("type");
    }
    /// <summary>Retrieve the <c>typeID</c> attribute of the item</summary>
    public static IAttribute TypeId(this IItem item)
    {
      return item.Attribute("typeID");
    }
    /// <summary>Retrieve the <c>where</c> attribute of the item</summary>
    public static IAttribute Where(this IItem item)
    {
      return item.Attribute("where");
    }
    #endregion

    #region "Item Properties"
    /// <summary>Retrieve the <c>classification</c> property of the item</summary>
    public static IProperty Classification(this IItem parent)
    {
      return parent.Property("classification");
    }
    /// <summary>Retrieve the <c>config_id</c> property of the item</summary>
    public static IProperty ConfigId(this IItem parent)
    {
      return parent.Property("config_id");
    }
    /// <summary>Retrieve the <c>created_by_id</c> property of the item</summary>
    public static IProperty CreatedById(this IItem parent)
    {
      return parent.Property("created_by_id");
    }
    /// <summary>Retrieve the <c>created_on</c> property of the item</summary>
    /// <example>
    /// <code lang="C#">
    /// // If the part was created after 2016-01-01, put the name of the creator in the description
    /// if (comp.CreatedOn().AsDateTime(DateTime.MaxValue) > new DateTime(2016, 1, 1))
    /// {
    ///     edits.Property("description").Set("Created by: " + comp.CreatedById().KeyedName().Value);
    /// }
    /// </code>
    /// </example>
    public static IProperty CreatedOn(this IItem parent)
    {
      return parent.Property("created_on");
    }
    /// <summary>Retrieve the <c>css</c> property of the item</summary>
    public static IProperty Css(this IItem parent)
    {
      return parent.Property("css");
    }
    /// <summary>Retrieve the <c>current_state</c> property of the item</summary>
    public static IProperty CurrentState(this IItem parent)
    {
      return parent.Property("current_state");
    }
    /// <summary>Retrieve the <c>generation</c> property of the item</summary>
    public static IProperty Generation(this IItem parent)
    {
      return parent.Property("generation");
    }
    /// <summary>Retrieve the <c>is_current</c> property of the item</summary>
    public static IProperty IsCurrent(this IItem parent)
    {
      return parent.Property("is_current");
    }
    /// <summary>Retrieve the <c>is_released</c> property of the item</summary>
    public static IProperty IsReleased(this IItem parent)
    {
      return parent.Property("is_released");
    }
    /// <summary>Retrieve the <c>keyed_name</c> property of the item</summary>
    public static IProperty KeyedName(this IItem parent)
    {
      return parent.Property("keyed_name");
    }
    /// <summary>Retrieve the <c>locked_by_id</c> property of the item</summary>
    public static IProperty LockedById(this IItem parent)
    {
      return parent.Property("locked_by_id");
    }
    /// <summary>Retrieve the <c>major_rev</c> property of the item</summary>
    public static IProperty MajorRev(this IItem parent)
    {
      return parent.Property("major_rev");
    }
    /// <summary>Retrieve the <c>managed_by_id</c> property of the item</summary>
    public static IProperty ManagedById(this IItem parent)
    {
      return parent.Property("managed_by_id");
    }
    /// <summary>Retrieve the <c>minor_rev</c> property of the item</summary>
    public static IProperty MinorRev(this IItem parent)
    {
      return parent.Property("minor_rev");
    }
    /// <summary>Retrieve the <c>modified_by_id</c> property of the item</summary>
    public static IProperty ModifiedById(this IItem parent)
    {
      return parent.Property("modified_by_id");
    }
    /// <summary>Retrieve the <c>modified_on</c> property of the item</summary>
    public static IProperty ModifiedOn(this IItem parent)
    {
      return parent.Property("modified_on");
    }
    /// <summary>Retrieve the <c>new_version</c> property of the item</summary>
    public static IProperty NewVersion(this IItem parent)
    {
      return parent.Property("new_version");
    }
    /// <summary>Retrieve the <c>not_lockable</c> property of the item</summary>
    public static IProperty NotLockable(this IItem parent)
    {
      return parent.Property("not_lockable");
    }
    /// <summary>Retrieve the <c>owned_by_id</c> property of the item</summary>
    public static IProperty OwnedById(this IItem parent)
    {
      return parent.Property("owned_by_id");
    }
    /// <summary>Retrieve the <c>permission_id</c> property of the item</summary>
    public static IProperty PermissionId(this IItem parent)
    {
      return parent.Property("permission_id");
    }
    /// <summary>Retrieve the <c>related_id</c> property of the item</summary>
    public static IProperty RelatedId(this IItem parent)
    {
      return parent.Property("related_id");
    }
    /// <summary>Retrieve the value of the <c>related_id</c> property as an Item</summary>
    public static IItem RelatedItem(this IItem parent)
    {
      return parent.Property("related_id").AsItem();
    }
    /// <summary>Retrieve the <c>state</c> property of the item</summary>
    public static IProperty State(this IItem parent)
    {
      return parent.Property("state");
    }
    /// <summary>Retrieve the <c>source_id</c> property of the item</summary>
    public static IProperty SourceId(this IItem parent)
    {
      return parent.Property("source_id");
    }
    /// <summary>Retrieve the value of the <c>source_id</c> property as an Item</summary>
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
    /// <summary>Retrieve the <c>condition</c> attribute of the property</summary>
    public static IReadOnlyAttribute Condition(this IReadOnlyProperty item)
    {
      return item.Attribute("condition");
    }
    /// <summary>Retrieve the <c>is_null</c> attribute of the property</summary>
    public static IReadOnlyAttribute IsNull(this IReadOnlyProperty item)
    {
      return item.Attribute("is_null");
    }
    /// <summary>Retrieve the <c>keyed_name</c> attribute of the property</summary>
    public static IReadOnlyAttribute KeyedName(this IReadOnlyProperty item)
    {
      return item.Attribute("keyed_name");
    }
    /// <summary>Retrieve the <c>type</c> attribute of the property</summary>
    public static IReadOnlyAttribute Type(this IReadOnlyProperty item)
    {
      return item.Attribute("type");
    }
    #endregion

    #region "Read Only Item Attributes"
    /// <summary>Retrieve the <c>action</c> attribute of the item</summary>
    public static IReadOnlyAttribute Action(this IReadOnlyItem item)
    {
      return item.Attribute("action");
    }
    /// <summary>Retrieve the <c>doGetItem</c> attribute of the item</summary>
    public static IReadOnlyAttribute DoGetItem(this IReadOnlyItem item)
    {
      return item.Attribute("doGetItem");
    }
    /// <summary>Retrieve the <c>idlist</c> attribute of the item</summary>
    public static IReadOnlyAttribute IdList(this IReadOnlyItem item)
    {
      return item.Attribute("idlist");
    }
    /// <summary>Retrieve the <c>maxRecords</c> attribute of the item</summary>
    public static IReadOnlyAttribute MaxRecords(this IReadOnlyItem item)
    {
      return item.Attribute("maxRecords");
    }
    /// <summary>Retrieve the <c>orderBy</c> attribute of the item</summary>
    public static IReadOnlyAttribute OrderBy(this IReadOnlyItem item)
    {
      return item.Attribute("orderBy");
    }
    /// <summary>Retrieve the <c>page</c> attribute of the item</summary>
    public static IReadOnlyAttribute Page(this IReadOnlyItem item)
    {
      return item.Attribute("page");
    }
    /// <summary>Retrieve the <c>pagesize</c> attribute of the item</summary>
    public static IReadOnlyAttribute PageSize(this IReadOnlyItem item)
    {
      return item.Attribute("pagesize");
    }
    /// <summary>Retrieve the <c>queryDate</c> attribute of the item</summary>
    public static IReadOnlyAttribute QueryDate(this IReadOnlyItem item)
    {
      return item.Attribute("queryDate");
    }
    /// <summary>Retrieve the <c>queryType</c> attribute of the item</summary>
    public static IReadOnlyAttribute QueryType(this IReadOnlyItem item)
    {
      return item.Attribute("queryType");
    }
    /// <summary>Retrieve the <c>related_expand</c> attribute of the item</summary>
    public static IReadOnlyAttribute RelatedExpand(this IReadOnlyItem item)
    {
      return item.Attribute("related_expand");
    }
    /// <summary>Retrieve the <c>select</c> attribute of the item</summary>
    public static IReadOnlyAttribute Select(this IReadOnlyItem item)
    {
      return item.Attribute("select");
    }
    /// <summary>Retrieve the <c>serverEvents</c> attribute of the item</summary>
    public static IReadOnlyAttribute ServerEvents(this IReadOnlyItem item)
    {
      return item.Attribute("serverEvents");
    }
    /// <summary>Retrieve the <c>type</c> attribute of the item</summary>
    public static IReadOnlyAttribute Type(this IReadOnlyItem item)
    {
      return item.Attribute("type");
    }
    /// <summary>Retrieve the <c>typeId</c> attribute of the item</summary>
    public static IReadOnlyAttribute TypeId(this IReadOnlyItem item)
    {
      return item.Attribute("typeId");
    }
    /// <summary>Retrieve the <c>where</c> attribute of the item</summary>
    public static IReadOnlyAttribute Where(this IReadOnlyItem item)
    {
      return item.Attribute("where");
    }
    #endregion

    #region "Read Only Item Properties"
    /// <summary>Retrieve the <c>classification</c> property of the item</summary>
    public static IReadOnlyProperty Classification(this IReadOnlyItem parent)
    {
      return parent.Property("classification");
    }
    /// <summary>Retrieve the <c>config_id</c> property of the item</summary>
    public static IReadOnlyProperty ConfigId(this IReadOnlyItem parent)
    {
      return parent.Property("config_id");
    }
    /// <summary>Retrieve the <c>created_by_id</c> property of the item</summary>
    public static IReadOnlyProperty CreatedById(this IReadOnlyItem parent)
    {
      return parent.Property("created_by_id");
    }
    /// <summary>Retrieve the <c>created_on</c> property of the item</summary>
    /// <example>
    /// <code lang="C#">
    /// // If the part was created after 2016-01-01, put the name of the creator in the description
    /// if (comp.CreatedOn().AsDateTime(DateTime.MaxValue) > new DateTime(2016, 1, 1))
    /// {
    ///     edits.Property("description").Set("Created by: " + comp.CreatedById().KeyedName().Value);
    /// }
    /// </code>
    /// </example>
    public static IReadOnlyProperty CreatedOn(this IReadOnlyItem parent)
    {
      return parent.Property("created_on");
    }
    /// <summary>Retrieve the <c>css</c> property of the item</summary>
    public static IReadOnlyProperty Css(this IReadOnlyItem parent)
    {
      return parent.Property("css");
    }
    /// <summary>Retrieve the <c>current_state</c> property of the item</summary>
    public static IReadOnlyProperty CurrentState(this IReadOnlyItem parent)
    {
      return parent.Property("current_state");
    }
    /// <summary>Retrieve the <c>generation</c> property of the item</summary>
    public static IReadOnlyProperty Generation(this IReadOnlyItem parent)
    {
      return parent.Property("generation");
    }
    /// <summary>Retrieve the <c>is_current</c> property of the item</summary>
    public static IReadOnlyProperty IsCurrent(this IReadOnlyItem parent)
    {
      return parent.Property("is_current");
    }
    /// <summary>Retrieve the <c>is_released</c> property of the item</summary>
    public static IReadOnlyProperty IsReleased(this IReadOnlyItem parent)
    {
      return parent.Property("is_released");
    }
    /// <summary>Retrieve the <c>keyed_name</c> property of the item</summary>
    public static IReadOnlyProperty KeyedName(this IReadOnlyItem parent)
    {
      return parent.Property("keyed_name");
    }
    /// <summary>Retrieve the <c>locked_by_id</c> property of the item</summary>
    public static IReadOnlyProperty LockedById(this IReadOnlyItem parent)
    {
      return parent.Property("locked_by_id");
    }
    /// <summary>Retrieve the <c>major_rev</c> property of the item</summary>
    public static IReadOnlyProperty MajorRev(this IReadOnlyItem parent)
    {
      return parent.Property("major_rev");
    }
    /// <summary>Retrieve the <c>managed_by_id</c> property of the item</summary>
    public static IReadOnlyProperty ManagedById(this IReadOnlyItem parent)
    {
      return parent.Property("managed_by_id");
    }
    /// <summary>Retrieve the <c>minor_rev</c> property of the item</summary>
    public static IReadOnlyProperty MinorRev(this IReadOnlyItem parent)
    {
      return parent.Property("minor_rev");
    }
    /// <summary>Retrieve the <c>modified_by_id</c> property of the item</summary>
    public static IReadOnlyProperty ModifiedById(this IReadOnlyItem parent)
    {
      return parent.Property("modified_by_id");
    }
    /// <summary>Retrieve the <c>modified_on</c> property of the item</summary>
    public static IReadOnlyProperty ModifiedOn(this IReadOnlyItem parent)
    {
      return parent.Property("modified_on");
    }
    /// <summary>Retrieve the <c>new_version</c> property of the item</summary>
    public static IReadOnlyProperty NewVersion(this IReadOnlyItem parent)
    {
      return parent.Property("new_version");
    }
    /// <summary>Retrieve the <c>not_lockable</c> property of the item</summary>
    public static IReadOnlyProperty NotLockable(this IReadOnlyItem parent)
    {
      return parent.Property("not_lockable");
    }
    /// <summary>Retrieve the <c>owned_by_id</c> property of the item</summary>
    public static IReadOnlyProperty OwnedById(this IReadOnlyItem parent)
    {
      return parent.Property("owned_by_id");
    }
    /// <summary>Retrieve the <c>permission_id</c> property of the item</summary>
    public static IReadOnlyProperty PermissionId(this IReadOnlyItem parent)
    {
      return parent.Property("permission_id");
    }
    /// <summary>Retrieve the <c>related_id</c> property of the item</summary>
    public static IReadOnlyProperty RelatedId(this IReadOnlyItem parent)
    {
      return parent.Property("related_id");
    }
    /// <summary>Retrieve the value of the <c>related_id</c> property as an Item</summary>
    public static IReadOnlyItem RelatedItem(this IReadOnlyItem parent)
    {
      return parent.Property("related_id").AsItem();
    }
    /// <summary>Retrieve the <c>state</c> property of the item</summary>
    public static IReadOnlyProperty State(this IReadOnlyItem parent)
    {
      return parent.Property("state");
    }
    /// <summary>Retrieve the <c>source_id</c> property of the item</summary>
    public static IReadOnlyProperty SourceId(this IReadOnlyItem parent)
    {
      return parent.Property("source_id");
    }
    /// <summary>Retrieve the value of the <c>source_id</c> property as an Item</summary>
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
