using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Provides metadata for an Aras file vault
  /// </summary>
  public class Vault
  {
    private CookieContainer _cookies = new CookieContainer();

    public AuthenticationSchemes Authentication { get; set; }
    public CookieContainer Cookies { get { return _cookies; } }
    public string Id { get; set; }
    public string Url { get; set; }

    private Vault(IReadOnlyItem i)
    {
      this.Id = i.Id();
      this.Url = i.Property("vault_url").Value;
      this.Authentication = AuthenticationSchemes.None;
    }

    private static Dictionary<string, Vault> _vaults = new Dictionary<string, Vault>();

    /// <summary>
    /// Creates a vault from metadata stored in an Aras Item
    /// </summary>
    public static Vault GetVault(IReadOnlyItem i)
    {
      if (i == null || !i.Exists) return null;
      Vault result;
      if (!_vaults.TryGetValue(i.Id(), out result))
      {
        result = new Vault(i);
        _vaults[i.Id()] = result;
      }
      return result;
    }
    /// <summary>
    /// Creates a vault from an ID
    /// </summary>
    public static Vault GetVault(string id)
    {
      Vault result;
      if (!_vaults.TryGetValue(id, out result)) return null;
      return result;
    }
  }
}
