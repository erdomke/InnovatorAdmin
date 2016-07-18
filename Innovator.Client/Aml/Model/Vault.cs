using Innovator.Client;
using System;

namespace Innovator.Client.Model
{
  ///<summary>Class for the item type Vault </summary>
  public class Vault : Item
  {
    protected Vault() { }
    public Vault(ElementFactory amlContext, params object[] content) : base(amlContext, content) { }
    /// <summary>Retrieve the <c>label</c> property of the item</summary>
    public IProperty_Text Label()
    {
      return this.Property("label");
    }
    /// <summary>Retrieve the <c>name</c> property of the item</summary>
    public IProperty_Text NameProp()
    {
      return this.Property("name");
    }
    /// <summary>Retrieve the <c>vault_url</c> property of the item</summary>
    public IProperty_Text VaultUrl()
    {
      return this.Property("vault_url");
    }
    /// <summary>Retrieve the <c>vault_url_pattern</c> property of the item</summary>
    public IProperty_Text VaultUrlPattern()
    {
      return this.Property("vault_url_pattern");
    }
  }
}