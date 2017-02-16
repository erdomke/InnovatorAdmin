using Innovator.Client;
using InnovatorAdmin.Connections;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin
{
  public static class ConnectionDataExtensions
  {
    public static string CalcMD5(string value)
    {
      using (var md5 = new MD5CryptoServiceProvider())
      {
        var result = new StringBuilder();
        var hash = md5.ComputeHash(Encoding.ASCII.GetBytes(value));
        for (var i = 0; i < hash.Length; i++)
        {
          result.AppendFormat("{0:x2}", hash[i]);
        }
        return result.ToString();
      }
    }
    public static IAsyncConnection ArasLogin(this ConnectionData credentials)
    {
      return ArasLogin(credentials, false).Value;
    }

    public static ICredentials ArasCredentials(this ConnectionData credentials)
    {
      if (credentials.Type == ConnectionType.Innovator)
      {
        switch (credentials.Authentication)
        {
          case Connections.Authentication.Anonymous:
            return new AnonymousCredentials(credentials.Database);
          case Connections.Authentication.Windows:
            return new WindowsCredentials(credentials.Database);
          default:
            return new ExplicitCredentials(credentials.Database, credentials.UserName, credentials.Password);
        }
      }
      return null;
    }
    public static IPromise<IAsyncConnection> ArasLogin(this ConnectionData credentials, bool async)
    {
      var cred = credentials.ArasCredentials();
      var prefs = new ConnectionPreferences();
      prefs.Headers.UserAgent = "InnovatorAdmin v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();
      var localePref = credentials.Params.FirstOrDefault(p => p.Name == "LOCALE");
      var tzPref = credentials.Params.FirstOrDefault(p => p.Name == "TIMEZONE_NAME");
      if (localePref != null)
        prefs.Headers.Locale = localePref.Value;
      if (tzPref != null)
        prefs.Headers.TimeZone = tzPref.Value;

      return Factory.GetConnection(credentials.Url
        , prefs, async)
      .Continue(c =>
      {
        return c.Login(cred, async)
          .Convert(u => (IAsyncConnection)c);
      });
    }
    public static void Explore(this ConnectionData conn)
    {
      if (conn.Type == ConnectionType.Innovator)
      {
        var arasUrl = conn.Url + "?bypass_logon_form=1&database=" + conn.Database + "&username=" + conn.UserName + "&password=" + CalcMD5(conn.Password);
        using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\IEXPLORE.EXE"))
        {
          if (key != null)
          {
            var iePath = (string)key.GetValue(null);
            System.Diagnostics.Process.Start(iePath, arasUrl);
          }
        }
      }
    }
  }
}
