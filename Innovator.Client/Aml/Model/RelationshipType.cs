using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type RelationshipType </summary>
  public class RelationshipType : Item
  {
    protected RelationshipType() { }
    public RelationshipType(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>auto_search</c> property of the item</summary>
    public IProperty_Boolean AutoSearch()
    {
      return this.Property("auto_search");
    }
    /// <summary>Retrieve the <c>behavior</c> property of the item</summary>
    public IProperty_Text Behavior()
    {
      return this.Property("behavior");
    }
    /// <summary>Retrieve the <c>copy_permissions</c> property of the item</summary>
    public IProperty_Boolean CopyPermissions()
    {
      return this.Property("copy_permissions");
    }
    /// <summary>Retrieve the <c>core</c> property of the item</summary>
    public IProperty_Boolean Core()
    {
      return this.Property("core");
    }
    /// <summary>Retrieve the <c>create_related</c> property of the item</summary>
    public IProperty_Boolean CreateRelated()
    {
      return this.Property("create_related");
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
    /// <summary>Retrieve the <c>grid_view</c> property of the item</summary>
    public IProperty_Text GridView()
    {
      return this.Property("grid_view");
    }
    /// <summary>Retrieve the <c>help_item</c> property of the item</summary>
    public IProperty_Item HelpItem()
    {
      return this.Property("help_item");
    }
    /// <summary>Retrieve the <c>help_url</c> property of the item</summary>
    public IProperty_Text HelpUrl()
    {
      return this.Property("help_url");
    }
    /// <summary>Retrieve the <c>hide_in_all</c> property of the item</summary>
    public IProperty_Boolean HideInAll()
    {
      return this.Property("hide_in_all");
    }
    /// <summary>Retrieve the <c>inc_rel_key_name</c> property of the item</summary>
    public IProperty_Boolean IncRelKeyName()
    {
      return this.Property("inc_rel_key_name");
    }
    /// <summary>Retrieve the <c>inc_related_key_name</c> property of the item</summary>
    public IProperty_Boolean IncRelatedKeyName()
    {
      return this.Property("inc_related_key_name");
    }
    /// <summary>Retrieve the <c>is_list_type</c> property of the item</summary>
    public IProperty_Boolean IsListType()
    {
      return this.Property("is_list_type");
    }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>max_occurs</c> property of the item</summary>
    public IProperty_Number MaxOccurs()
    {
      return this.Property("max_occurs");
    }
    /// <summary>Retrieve the <c>min_occurs</c> property of the item</summary>
    public IProperty_Number MinOccurs()
    {
      return this.Property("min_occurs");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>new_show_related</c> property of the item</summary>
    public IProperty_Boolean NewShowRelated()
    {
      return this.Property("new_show_related");
    }
    /// <summary>Retrieve the <c>related_notnull</c> property of the item</summary>
    public IProperty_Boolean RelatedNotnull()
    {
      return this.Property("related_notnull");
    }
    /// <summary>Retrieve the <c>related_option</c> property of the item</summary>
    public IProperty_Text RelatedOption()
    {
      return this.Property("related_option");
    }
    /// <summary>Retrieve the <c>relationship_id</c> property of the item</summary>
    public IProperty_Item RelationshipId()
    {
      return this.Property("relationship_id");
    }
    /// <summary>Retrieve the <c>sort_order</c> property of the item</summary>
    public IProperty_Number SortOrder()
    {
      return this.Property("sort_order");
    }
  }
}