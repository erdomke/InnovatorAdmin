using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using Innovator.Server;

namespace InnovatorAdmin.Plugin
{
  public class PluginContext : IPluginContext
  {
    private readonly Action<int, string> _progressCallback;

    public IReadOnlyItem Item { get; }
    public IServerConnection Conn { get; }
    public IEnumerable<ICredentials> Credentials { get; }

    public PluginContext(PluginConnection conn, Action<int, string> progressCallback, IEnumerable<ICredentials> credentials)
    {
      Conn = conn;
      Credentials = credentials;
      var items = conn.AmlContext.FromXml(conn.OriginalRequest).Items().ToArray();
      if (items.Length == 1)
        Item = items[0];
      else
        Item = Innovator.Client.Item.GetNullItem<IReadOnlyItem>();
      _progressCallback = progressCallback;
    }

    public void RecordProgress(int percent, string message)
    {
      _progressCallback?.Invoke(percent, message);
    }
  }
}
