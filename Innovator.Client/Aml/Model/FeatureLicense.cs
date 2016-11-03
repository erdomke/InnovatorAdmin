using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Feature License </summary>
  public class FeatureLicense : Item
  {
    protected FeatureLicense() { }
    public FeatureLicense(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    static FeatureLicense() { Innovator.Client.Item.AddNullItem<FeatureLicense>(new FeatureLicense { _attr = ElementAttribute.ReadOnly | ElementAttribute.Null }); }

    /// <summary>Retrieve the <c>activation_key</c> property of the item</summary>
    public IProperty_Text ActivationKey()
    {
      return this.Property("activation_key");
    }
    /// <summary>Retrieve the <c>additional_license_data</c> property of the item</summary>
    public IProperty_Text AdditionalLicenseData()
    {
      return this.Property("additional_license_data");
    }
    /// <summary>Retrieve the <c>available_to</c> property of the item</summary>
    public IProperty_Item<Identity> AvailableTo()
    {
      return this.Property("available_to");
    }
    /// <summary>Retrieve the <c>concurrent_user_count</c> property of the item</summary>
    public IProperty_Number ConcurrentUserCount()
    {
      return this.Property("concurrent_user_count");
    }
    /// <summary>Retrieve the <c>database_engine_edition</c> property of the item</summary>
    public IProperty_Text DatabaseEngineEdition()
    {
      return this.Property("database_engine_edition");
    }
    /// <summary>Retrieve the <c>database_server_name</c> property of the item</summary>
    public IProperty_Text DatabaseServerName()
    {
      return this.Property("database_server_name");
    }
    /// <summary>Retrieve the <c>domain</c> property of the item</summary>
    public IProperty_Text Domain()
    {
      return this.Property("domain");
    }
    /// <summary>Retrieve the <c>enforcement_mode</c> property of the item</summary>
    public IProperty_Text EnforcementMode()
    {
      return this.Property("enforcement_mode");
    }
    /// <summary>Retrieve the <c>expiration_date</c> property of the item</summary>
    public IProperty_Text ExpirationDate()
    {
      return this.Property("expiration_date");
    }
    /// <summary>Retrieve the <c>feature</c> property of the item</summary>
    public IProperty_Text Feature()
    {
      return this.Property("feature");
    }
    /// <summary>Retrieve the <c>framework_license_key</c> property of the item</summary>
    public IProperty_Text FrameworkLicenseKey()
    {
      return this.Property("framework_license_key");
    }
    /// <summary>Retrieve the <c>innovator_server_locale</c> property of the item</summary>
    public IProperty_Text InnovatorServerLocale()
    {
      return this.Property("innovator_server_locale");
    }
    /// <summary>Retrieve the <c>innovator_server_name</c> property of the item</summary>
    public IProperty_Text InnovatorServerName()
    {
      return this.Property("innovator_server_name");
    }
    /// <summary>Retrieve the <c>issued_to</c> property of the item</summary>
    public IProperty_Text IssuedTo()
    {
      return this.Property("issued_to");
    }
    /// <summary>Retrieve the <c>license_data</c> property of the item</summary>
    public IProperty_Text LicenseData()
    {
      return this.Property("license_data");
    }
    /// <summary>Retrieve the <c>license_description</c> property of the item</summary>
    public IProperty_Text LicenseDescription()
    {
      return this.Property("license_description");
    }
    /// <summary>Retrieve the <c>login</c> property of the item</summary>
    public IProperty_Text Login()
    {
      return this.Property("login");
    }
    /// <summary>Retrieve the <c>mac_address</c> property of the item</summary>
    public IProperty_Text MacAddress()
    {
      return this.Property("mac_address");
    }
    /// <summary>Retrieve the <c>named_user_list</c> property of the item</summary>
    public IProperty_Text NamedUserList()
    {
      return this.Property("named_user_list");
    }
    /// <summary>Retrieve the <c>password</c> property of the item</summary>
    public IProperty_Text Password()
    {
      return this.Property("password");
    }
    /// <summary>Retrieve the <c>secure_id</c> property of the item</summary>
    public IProperty_Text SecureId()
    {
      return this.Property("secure_id");
    }
    /// <summary>Retrieve the <c>start_date</c> property of the item</summary>
    public IProperty_Text StartDate()
    {
      return this.Property("start_date");
    }
    /// <summary>Retrieve the <c>total_users</c> property of the item</summary>
    public IProperty_Number TotalUsers()
    {
      return this.Property("total_users");
    }
  }
}