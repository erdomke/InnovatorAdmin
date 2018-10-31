using CommandLine;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  class SharedOptions
  {
    [Option('s', "server", HelpText = "server's URL (e.g. server=http://localhost/InnovatorServer)", Required = true)]
    public string Url { get; set; }

    [Option('d', "database", HelpText = "database's name (e.g. database=dbWithoutSolution)", Required = true)]
    public string Database { get; set; }

    [Option('l', "login", HelpText = "user's login (e.g. login=root); NOTE: login must have root or admin privileges")]
    public string Username { get; set; }

    [Option('p', "password", HelpText = "user's password (e.g. password=xxxx )")]
    public string Password { get; set; }

    [Option('k', "package", HelpText = "Path to package file containing the items to import/export")]
    public string Package { get; set; }

    public async Task<IAsyncConnection> GetConnection()
    {
      var conn = await Factory.GetConnection(new ConnectionPreferences()
      {
        Headers =
        {
          UserAgent = "InnovatorAdmin.Cmd v" + Assembly.GetExecutingAssembly().GetName().Version.ToString()
        },
        Url = Url,
      }, true).ConfigureAwait(false);
      await conn.Login(GetCredentials(), true).ConfigureAwait(false);
      return conn;
    }

    public ICredentials GetCredentials()
    {
      if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(Username))
        return new WindowsCredentials(Database);
      if (IsGuid(Password))
        return new ExplicitHashCredentials(Database, Username, Password);
      return new ExplicitCredentials(Database, Username, Password);
    }

    private static bool IsGuid(string value)
    {
      if (value?.Length != 32) return false;
      for (var i = 0; i < value.Length; i++)
      {
        switch (value[i])
        {
          case '0':
          case '1':
          case '2':
          case '3':
          case '4':
          case '5':
          case '6':
          case '7':
          case '8':
          case '9':
          case 'A':
          case 'B':
          case 'C':
          case 'D':
          case 'E':
          case 'F':
          case 'a':
          case 'b':
          case 'c':
          case 'd':
          case 'e':
          case 'f':
            break;
          default:
            return false;
        }
      }
      return true;
    }
  }
}
