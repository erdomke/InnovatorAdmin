using Innovator.Client;
using Innovator.Client.Connection;
using Innovator.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Plugin
{
  public class PluginConnection : IServerConnection, IAsyncConnection
  {
    private readonly IAsyncConnection _base;
    private readonly Cache _cache = new Cache();

    /// <summary>
    /// Gets the in-memory application-wide cache.
    /// </summary>
    /// <value>
    /// The application cache.
    /// </value>
    public IServerCache ApplicationCache => _cache;

    /// <summary>
    /// Gets the string representing original AML request which eventually
    /// resulted in the execution of the current code
    /// </summary>
    /// <value>
    /// The original AML request.
    /// </value>
    public string OriginalRequest { get; }

    /// <summary>
    /// Gets the information about the current user's permissions.
    /// </summary>
    /// <value>
    /// The permissions.
    /// </value>
    public IServerPermissions Permissions { get; }

    /// <summary>
    /// Gets the in-memory request-specific cache.
    /// </summary>
    /// <value>
    /// The request cache.
    /// </value>
    public IServerCache RequestState => _cache;

    /// <summary>
    /// Gets the requested URL.
    /// </summary>
    /// <value>
    /// The requested URL.
    /// </value>
    public string RequestUrl => null;

    /// <summary>
    /// Gets the in-memory session-specific cache.
    /// </summary>
    /// <value>
    /// The session cache.
    /// </value>
    public IServerCache SessionCache => _cache;

    /// <summary>
    /// AML context used for creating AML objects and formatting AML statements
    /// </summary>
    public ElementFactory AmlContext => _base.AmlContext;

    /// <summary>
    /// Name of the connected database
    /// </summary>
    public string Database => _base.Database;

    /// <summary>
    /// ID of the authenticated user
    /// </summary>
    public string UserId => _base.UserId;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConnection"/> class.
    /// </summary>
    /// <param name="conn">The connection.</param>
    /// <param name="original">The original.</param>
    public PluginConnection(IAsyncConnection conn, string original)
    {
      _base = conn;
      OriginalRequest = original;
      Permissions = new PermissionInfo(conn);
    }

    /// <summary>
    /// Creates an upload request used for uploading files to the server
    /// </summary>
    /// <returns>
    /// A new upload request used for uploading files to the server
    /// </returns>
    public UploadCommand CreateUploadCommand()
    {
      return _base.CreateUploadCommand();
    }

    /// <summary>
    /// Gets the HTTP header by name.
    /// </summary>
    /// <param name="name">The HTTP name.</param>
    /// <returns>
    /// The value of the HTTP header
    /// </returns>
    public string GetHeader(string name)
    {
      if (string.Equals(name, "DATABASE", StringComparison.OrdinalIgnoreCase))
      {
        return _base.Database;
      }
      else if (_base is IArasConnection conn)
      {
        var result = default(string);
        conn.SetDefaultHeaders((h, v) =>
        {
          if (string.Equals(h, name, StringComparison.OrdinalIgnoreCase))
            result = v;
        });
        return result;
      }
      return null;
    }

    /// <summary>
    /// Hashes the credentials for use with logging in or workflow voting
    /// </summary>
    /// <param name="credentials">The credentials.</param>
    /// <returns>
    /// Hashed credentials
    /// </returns>
    public ExplicitHashCredentials HashCredentials(ICredentials credentials)
    {
      return _base.HashCredentials(credentials);
    }

    /// <summary>
    /// Hashes the credentials for use with logging in or workflow voting
    /// </summary>
    /// <param name="credentials">The credentials.</param>
    /// <param name="async">Whether to perform this action asynchronously</param>
    /// <returns>
    /// A promise to return hashed credentials
    /// </returns>
    public IPromise<ExplicitHashCredentials> HashCredentials(ICredentials credentials, bool async)
    {
      return _base.HashCredentials(credentials, async);
    }

    /// <summary>
    /// Expands a relative URL to a full URL
    /// </summary>
    /// <param name="relativeUrl">The relative URL</param>
    /// <returns>
    /// A full URL relative to the connection
    /// </returns>
    public string MapClientUrl(string relativeUrl)
    {
      return _base.MapClientUrl(relativeUrl);
    }

    /// <summary>
    /// Calls a SOAP action asynchronously
    /// </summary>
    /// <param name="request">Request AML and possibly files <see cref="T:Innovator.Client.UploadCommand" /></param>
    /// <returns>
    /// An XML SOAP response as a string
    /// </returns>
    public Stream Process(Command request)
    {
      return _base.Process(request);
    }

    /// <summary>
    /// Calls a SOAP action asynchronously
    /// </summary>
    /// <param name="request">Request AML and possibly files <see cref="T:Innovator.Client.UploadCommand" /></param>
    /// <param name="async">Whether to perform this action asynchronously</param>
    /// <returns>
    /// A promise to return an XML SOAP response as a <see cref="T:System.IO.Stream" />
    /// </returns>
    public IPromise<Stream> Process(Command request, bool async)
    {
      return _base.Process(request, async);
    }

    private class PermissionInfo : IServerPermissions
    {
      private readonly IConnection _conn;
      private HashSet<string> _identities;

      public bool IsRootOrAdmin
      {
        get
        {
          Identities();
          return _identities.Contains("6B14D33C4A7D41C188CCF2BC15BD01A3")
            || _identities.Contains("2618D6F5A90949BAA7E920D1B04C7EE1");
        }
      }

      public PermissionInfo(IConnection conn)
      {
        _conn = conn;
      }

      public IDisposable Escalate(params string[] identNames)
      {
        throw new NotSupportedException();
      }

      public IEnumerable<string> Identities()
      {
        return _identities
          ?? (_identities = new HashSet<string>(_conn.Apply(new Command("<Item/>").WithAction(CommandAction.GetIdentityList))
            .Value.Split(',')));
      }

      public IEnumerable<string> Identities(string userId)
      {
        throw new NotSupportedException();
      }
    }

    private class Cache : IServerCache
    {
      private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

      public object this[string key]
      {
        get
        {
          if (_cache.TryGetValue(key, out var result))
            return result;
          return null;
        }
        set
        {
          _cache[key] = value;
        }
      }

      public T Get<T>(string key)
      {
        return (T)this[key];
      }
    }
  }
}
