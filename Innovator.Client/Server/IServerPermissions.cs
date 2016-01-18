using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public interface IServerPermissions
  {
    bool IsRootOrAdmin { get; }
    IEnumerable<string> Identities();
    IEnumerable<string> Identities(string userId);
    IDisposable Escalate(string identName);
  }
}
