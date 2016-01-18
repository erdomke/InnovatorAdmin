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
    string IpAddress { get; }
    string OriginalRequest { get; }
    IServerPermissions Permissions { get; }
    IServerCache SessionCache { get; }
    string UserAgent { get; }

    IEnumerable<IReadOnlyItem> ApplySelect(string sql, string typeName = null);
    IConnection NewConnection();
  }
}
