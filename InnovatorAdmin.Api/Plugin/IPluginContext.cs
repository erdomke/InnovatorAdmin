using Innovator.Client;
using Innovator.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Plugin
{
  public interface IPluginContext : ISingleItemContext
  {
    IEnumerable<ICredentials> Credentials { get; }
    void RecordProgress(int percent, string message);
  }
}
