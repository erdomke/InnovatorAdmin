using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public interface IServerConnection : IConnection
  {
    IServerCache ApplicationCache { get; }
    string OriginalRequest { get; }
    IServerPermissions Permissions { get; }
    string RequestUrl { get; }
    IServerCache SessionCache { get; }

    string GetHeader(string name);
  }
}
