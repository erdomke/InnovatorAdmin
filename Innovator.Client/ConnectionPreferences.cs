using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class ConnectionPreferences
  {
    public ITokenStore CredentialStore { get; set; }
    public Connection.IHttpService HttpService { get; set; }
    public string Locale { get; set; }
    public SessionPolicy SessionPolicy { get; set; }
    public string TimeZone { get; set; }
    public string UserAgent { get; set; }
    
    public ConnectionPreferences()
    {
      this.SessionPolicy = SessionPolicy.PrivateNetwork;
    }
  }
}
