using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Powershell
{
  public class SavedConnection
  {
    private ConnectionPreferences _prefs;

    public string Database { get { return _prefs.Credentials.Database; } }
    public string Name { get { return _prefs.Name; } }
    public string Url { get { return _prefs.Url; } }
    public string Username
    {
      get
      {
        return (_prefs.Credentials as ExplicitCredentials)?.Username
          ?? (_prefs.Credentials as ExplicitHashCredentials)?.Username;
      }
    }
    private ConnectionPreferences Preferences { get { return _prefs; } }

    public SavedConnection(ConnectionPreferences prefs)
    {
      _prefs = prefs;
    }
  }
}
