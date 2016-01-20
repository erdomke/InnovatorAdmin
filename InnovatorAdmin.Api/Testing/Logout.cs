using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;

namespace InnovatorAdmin.Testing
{
  public class Logout : ICommand
  {
    public string Comment { get; set; }

    public async Task Run(TestContext context)
    {
      var conn = context.PopConnection();
      if (context.Connection == null)
      {
        context.PushConnection(conn);
      }
      else
      {
        var remote = conn as IRemoteConnection;
        if (remote != null)
          remote.Logout(false, true);
      }
    }
  }
}
