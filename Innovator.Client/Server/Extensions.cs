using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public static class Extensions
  {
    public static IDisposable AsAdmin(this IServerConnection conn)
    {
      return conn.Permissions.Escalate("Administrators");
    }
    public static IDisposable AsArasPlm(this IServerConnection conn)
    {
      return conn.Permissions.Escalate("ArasPlm");
    }
  }
}
