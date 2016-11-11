using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type SPDocumentLibraryDefinition </summary>
  public class SPDocumentLibraryDefinition : Item
  {
    protected SPDocumentLibraryDefinition() { }
    public SPDocumentLibraryDefinition(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static SPDocumentLibraryDefinition() { Innovator.Client.Item.AddNullItem<SPDocumentLibraryDefinition>(new SPDocumentLibraryDefinition { _attr = ElementAttributes.ReadOnly | ElementAttributes.Null }); }

    /// <summary>Retrieve the <c>authentication_type</c> property of the item</summary>
    public IProperty_Text AuthenticationType()
    {
      return this.Property("authentication_type");
    }
    /// <summary>Retrieve the <c>doc_type_label</c> property of the item</summary>
    public IProperty_Text DocTypeLabel()
    {
      return this.Property("doc_type_label");
    }
    /// <summary>Retrieve the <c>doc_type_label_plural</c> property of the item</summary>
    public IProperty_Text DocTypeLabelPlural()
    {
      return this.Property("doc_type_label_plural");
    }
    /// <summary>Retrieve the <c>doc_type_name</c> property of the item</summary>
    public IProperty_Text DocTypeName()
    {
      return this.Property("doc_type_name");
    }
    /// <summary>Retrieve the <c>sp_doclib_id</c> property of the item</summary>
    public IProperty_Text SpDoclibId()
    {
      return this.Property("sp_doclib_id");
    }
    /// <summary>Retrieve the <c>sp_doclib_name</c> property of the item</summary>
    public IProperty_Text SpDoclibName()
    {
      return this.Property("sp_doclib_name");
    }
    /// <summary>Retrieve the <c>sp_domain</c> property of the item</summary>
    public IProperty_Text SpDomain()
    {
      return this.Property("sp_domain");
    }
    /// <summary>Retrieve the <c>sp_password</c> property of the item</summary>
    public IProperty_Text SpPassword()
    {
      return this.Property("sp_password");
    }
    /// <summary>Retrieve the <c>sp_site_title</c> property of the item</summary>
    public IProperty_Text SpSiteTitle()
    {
      return this.Property("sp_site_title");
    }
    /// <summary>Retrieve the <c>sp_site_url</c> property of the item</summary>
    public IProperty_Text SpSiteUrl()
    {
      return this.Property("sp_site_url");
    }
    /// <summary>Retrieve the <c>sp_user</c> property of the item</summary>
    public IProperty_Text SpUser()
    {
      return this.Property("sp_user");
    }
  }
}