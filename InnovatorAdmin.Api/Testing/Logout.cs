using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;

namespace InnovatorAdmin.Testing
{
  /// <summary>
  /// Command to logout of the current connection
  /// </summary>
  public class Logout : ICommand
  {
    /// <summary>
    /// Comment preceding the command in the script
    /// </summary>
    public string Comment { get; set; }

    /// <summary>
    /// Code for executing the command
    /// </summary>
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

    /// <summary>
    /// Visit this object for the purposes of rendering it to an output
    /// </summary>
    public void Visit(ITestVisitor visitor)
    {
      visitor.Visit(this);
    }
  }
}
