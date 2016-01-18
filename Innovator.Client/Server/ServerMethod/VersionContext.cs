using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client;

namespace Innovator.Server
{
  public class VersionContext : IVersionContext
  {
    private IServerConnection _conn;
    private IReadOnlyItem _oldVersion;
    private bool _newLoaded;
    private IReadOnlyItem _newVersion;

    public IReadOnlyItem OldVersion
    {
      get { return _oldVersion; }
    }

    public IReadOnlyItem NewVersion
    {
      get 
      {
        EnsureNewVersion();
        return _newVersion;
      }
    }

    public Action<IItem> QueryDefaults { get; set; }

    public IServerConnection Conn
    {
      get { return _conn; }
    }

    public VersionContext(IServerConnection conn, IReadOnlyItem item)
    {
      _conn = conn;
      _oldVersion = item;
    }

    private void EnsureNewVersion()
    {
      if (!_newLoaded)
      {
        _newLoaded = true;
        var props = _oldVersion.LazyMap(_conn, i => new
        {
          ConfigId = i.ConfigId().Value,
          Generation = i.Generation().AsInt()
        });

        var aml = Conn.AmlContext;
        var query = aml.Item(_oldVersion.Type(), aml.Action("get"),
          aml.ConfigId(props.ConfigId),
          aml.Generation(props.Generation + 1)
        );
        if (QueryDefaults != null) QueryDefaults.Invoke(query);
        _newVersion = query.Apply(Conn).AssertItem();
      }
    }
  }
}
