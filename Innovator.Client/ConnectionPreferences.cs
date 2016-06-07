using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Preferences for connection behavior
  /// </summary>
  public class ConnectionPreferences
  {
    /// <summary>
    /// Allow a connection to a URL with a mapping file to send login requests to an authentication
    /// service
    /// </summary>
    public bool AllowAuthPreCheck { get; set; }
    public ITokenStore CredentialStore { get; set; }
    public Connection.IHttpService HttpService { get; set; }
    /// <summary>
    /// Locale to use when logging in
    /// </summary>
    public string Locale { get; set; }
    public SessionPolicy SessionPolicy { get; set; }
    /// <summary>
    /// Time zone to use with each request
    /// </summary>
    public string TimeZone { get; set; }
    /// <summary>
    /// User agent string to send with each request
    /// </summary>
    public string UserAgent { get; set; }

    public ConnectionPreferences()
    {
      this.SessionPolicy = SessionPolicy.PrivateNetwork;
      this.AllowAuthPreCheck = true;
    }
  }
}
