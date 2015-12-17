using Innovator.Client;
using InnovatorAdmin.Connections;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public static IPromise<IAsyncConnection> ArasLogin(this ConnectionData credentials, bool async)
    {
      ICredentials cred;
      switch (credentials.Authentication)
      {
        case Authentication.Anonymous:
          cred = new AnonymousCredentials(credentials.Database);
          break;
        case Authentication.Windows:
          cred = new WindowsCredentials(credentials.Database);
          break;
        default:
          cred = new ExplicitCredentials(credentials.Database, credentials.UserName, credentials.Password);
          break;
      }

      return Factory.GetConnection(credentials.Url
        , new ConnectionPreferences() { UserAgent = "InnovatorAdmin" }
        , async)
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
