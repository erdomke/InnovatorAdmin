using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type ItemType </summary>
  public class ItemType : Item
  {
    protected ItemType() { }
    public ItemType(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static ItemType() { Innovator.Client.Item.AddNullItem<ItemType>(new ItemType { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>allow_private_permission</c> property of the item</summary>
    public IProperty_Boolean AllowPrivatePermission()
    {
      return this.Property("allow_private_permission");
    }
    /// <summary>Retrieve the <c>auto_search</c> property of the item</summary>
    public IProperty_Boolean AutoSearch()
    {
      return this.Property("auto_search");
    }
    /// <summary>Retrieve the <c>cache_query</c> property of the item</summary>
    public IProperty_Text CacheQuery()
    {
      return this.Property("cache_query");
    }
    /// <summary>Retrieve the <c>class_structure</c> property of the item</summary>
    public IProperty_Text ClassStructure()
    {
      return this.Property("class_structure");
    }
    /// <summary>Retrieve the <c>close_icon</c> property of the item</summary>
    public IProperty_Text CloseIcon()
    {
      return this.Property("close_icon");
    }
    /// <summary>Retrieve the <c>core</c> property of the item</summary>
    public IProperty_Boolean Core()
    {
      return this.Property("core");
    }
    /// <summary>Retrieve the <c>default_page_size</c> property of the item</summary>
    public IProperty_Number DefaultPageSize()
    {
      return this.Property("default_page_size");
    }
    /// <summary>Retrieve the <c>description</c> property of the item</summary>
    public IProperty_Text Description()
    {
      return this.Property("description");
    }
    /// <summary>Retrieve the <c>enforce_discovery</c> property of the item</summary>
    public IProperty_Boolean EnforceDiscovery()
    {
      return this.Property("enforce_discovery");
    }
    /// <summary>Retrieve the <c>help_item</c> property of the item</summary>
    public IProperty_Item<Help> HelpItem()
    {
      return this.Property("help_item");
    }
    /// <summary>Retrieve the <c>help_url</c> property of the item</summary>
    public IProperty_Text HelpUrl()
    {
      return this.Property("help_url");
    }
    /// <summary>Retrieve the <c>hide_where_used</c> property of the item</summary>
    public IProperty_Boolean HideWhereUsed()
    {
      return this.Property("hide_where_used");
    }
    /// <summary>Retrieve the <c>history_template</c> property of the item</summary>
    public IProperty_Item<HistoryTemplate> HistoryTemplate()
    {
      return this.Property("history_template");
    }
    /// <summary>Retrieve the <c>implementation_ddl</c> property of the item</summary>
    public IProperty_Text ImplementationDdl()
    {
      return this.Property("implementation_ddl");
    }
    /// <summary>Retrieve the <c>implementation_type</c> property of the item</summary>
    public IProperty_Text ImplementationType()
    {
      return this.Property("implementation_type");
    }
    /// <summary>Retrieve the <c>instance_data</c> property of the item</summary>
    public IProperty_Text InstanceData()
    {
      return this.Property("instance_data");
    }
    /// <summary>Retrieve the <c>is_dependent</c> property of the item</summary>
    public IProperty_Boolean IsDependent()
    {
      return this.Property("is_dependent");
    }
    /// <summary>Retrieve the <c>is_relationship</c> property of the item</summary>
    public IProperty_Boolean IsRelationship()
    {
      return this.Property("is_relationship");
    }
    /// <summary>Retrieve the <c>is_versionable</c> property of the item</summary>
    public IProperty_Boolean IsVersionable()
    {
      return this.Property("is_versionable");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>label_plural</c> property of the item</summary>
    public IProperty_Text LabelPlural()
    {
      return this.Property("label_plural");
    }
    /// <summary>Retrieve the <c>large_icon</c> property of the item</summary>
    public IProperty_Text LargeIcon()
    {
      return this.Property("large_icon");
    }
    /// <summary>Retrieve the <c>manual_versioning</c> property of the item</summary>
    public IProperty_Text ManualVersioning()
    {
      return this.Property("manual_versioning");
    }
    /// <summary>Retrieve the <c>maxrecords</c> property of the item</summary>
    public IProperty_Number Maxrecords()
    {
      return this.Property("maxrecords");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>open_icon</c> property of the item</summary>
    public IProperty_Text OpenIcon()
    {
      return this.Property("open_icon");
    }
    /// <summary>Retrieve the <c>revisions</c> property of the item</summary>
    public IProperty_Item<Revision> Revisions()
    {
      return this.Property("revisions");
    }
    /// <summary>Retrieve the <c>show_parameters_tab</c> property of the item</summary>
    public IProperty_Text ShowParametersTab()
    {
      return this.Property("show_parameters_tab");
    }
    /// <summary>Retrieve the <c>structure_view</c> property of the item</summary>
    public IProperty_Text StructureView()
    {
      return this.Property("structure_view");
    }
    /// <summary>Retrieve the <c>unlock_on_logout</c> property of the item</summary>
    public IProperty_Boolean UnlockOnLogout()
    {
      return this.Property("unlock_on_logout");
    }
    /// <summary>Retrieve the <c>use_src_access</c> property of the item</summary>
    public IProperty_Boolean UseSrcAccess()
    {
      return this.Property("use_src_access");
    }
  }
}