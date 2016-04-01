using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  class MappedConnection : IRemoteConnection
  {
    private IRemoteConnection _current;
    private IEnumerable<ServerMapping> _mappings;

    public ElementFactory AmlContext { get { return _current.AmlContext; } }
    public string Database { get { return _current.Database; } }
    public Uri Url { get { return _current.Url; } }
    public string UserId { get { return _current.UserId; } }

    public MappedConnection(IEnumerable<ServerMapping> mappings)
    {
      _mappings = mappings;
    }

    public UploadCommand CreateUploadCommand()
    {
      return _current.CreateUploadCommand();
    }

    public void DefaultSettings(Action<IHttpRequest> settings)
    {
      _current.DefaultSettings(settings);
    }

    public void Dispose()
    {
      _current.Dispose();
    }

    public IEnumerable<string> GetDatabases()
    {
      return _mappings.SelectMany(s => s.Databases);
    }

    public void Login(ICredentials credentials)
    {
      var mapping = _mappings.First(m => m.Databases.Contains(credentials.Database));
      _current = mapping.Connection;
      _current.Login(credentials);
    }

    public IPromise<string> Login(ICredentials credentials, bool async)
    {
      var mapping = _mappings.First(m => m.Databases.Contains(credentials.Database));
      _current = mapping.Connection;
      return _current.Login(credentials, async);
    }

    public void Logout(bool unlockOnLogout)
    {
      _current.Logout(unlockOnLogout);
      _current = null;
    }

    public void Logout(bool unlockOnLogout, bool async)
    {
      _current.Logout(unlockOnLogout, async);
      _current = null;
    }

    public string MapClientUrl(string relativeUrl)
    {
      return _current.MapClientUrl(relativeUrl);
    }

    public Stream Process(Command request)
    {
      return _current.Process(request);
    }

    public IPromise<Stream> Process(Command request, bool async)
    {
      return _current.Process(request, async);
    }
  }
}
