using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;

namespace InnovatorAdmin.Testing
{
  public class Login : ICommand
  {
    public string Url { get; set; }
    public string Database { get; set; }
    public string UserName { get; set; }
    public SecureToken Password { get; set; }

    public string Comment { get; set; }

    public async Task Run(TestContext context)
    {
      var prefs = new ConnectionPreferences() { UserAgent = "InnovatorAdmin UnitTest" };
      var conn = await Factory.GetConnection(this.Url, prefs, true).ToTask();
      var cred = string.IsNullOrEmpty(UserName) || Password.IsNullOrEmpty()
        ? (ICredentials)new WindowsCredentials(this.Database)
        : new ExplicitCredentials(this.Database, this.UserName, this.Password);
      await conn.Login(cred, true).ToTask();
      context.PushConnection(conn);
    }
  }
}
