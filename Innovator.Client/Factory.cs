using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Innovator.Client.Connection;
using System.Xml;
using System.Xml.Linq;
using System.Net;

namespace Innovator.Client
{
  /// <summary>
  /// Class for generating connection to an Aras Innovator instance
  /// </summary>
  /// <example>
  /// <code lang="C#">
  /// using Innovator.Client;
  ///
  /// var conn = Factory.GetConnection("URL", "USER_AGENT");
  /// conn.Login(new ExplicitCredentials("DATABASE", "USER_NAME", "PASSWORD"));
  /// </code>
  /// </example>
  public static class Factory
  {
    private static MemoryCache<string, byte[]> _imageCache = new MemoryCache<string, byte[]>();
    internal static Func<Connection.IHttpService> DefaultService { get; set; }

    internal static MemoryCache<string, byte[]> ImageCache
    {
      get { return _imageCache; }
    }

    static Factory()
    {
      DefaultService = () => new Connection.DefaultHttpService();
    }

    /// <summary>
    /// How many images to buffer in memory when downloading image files.  This cache is used
    /// by the <see cref="ItemExtensions.AsFile(IReadOnlyProperty, IConnection, bool)"/>
    /// extension method.
    /// </summary>
    public static long ImageBufferSize
    {
      get { return _imageCache.MaxSize; }
      set { _imageCache.MaxSize = value; }
    }

    /// <summary>
    /// Gets an HTTP connection to an innovator instance (or proxy) at the given URL
    /// </summary>
    /// <param name="url">URL of the innovator instance (or proxy)</param>
    /// <param name="userAgent">User agent string to use for the connection</param>
    /// <returns>A connection object</returns>
    /// <example>
    /// <code lang="C#">
    /// using Innovator.Client;
    ///
    /// var conn = Factory.GetConnection("URL", "USER_AGENT");
    /// conn.Login(new ExplicitCredentials("DATABASE", "USER_NAME", "PASSWORD"));
    /// </code>
    /// </example>
    public static IRemoteConnection GetConnection(string url, string userAgent)
    {
      return GetConnection(url, new ConnectionPreferences() { UserAgent = userAgent }, false).Value;
    }

    /// <summary>
    /// Gets an HTTP connection to an innovator instance (or proxy) at the given URL
    /// </summary>
    /// <param name="url">URL of the innovator instance (or proxy)</param>
    /// <param name="preferences">Object containing preferences for the connection</param>
    /// <returns>A connection object</returns>
    public static IRemoteConnection GetConnection(string url, ConnectionPreferences preferences)
    {
      return GetConnection(url, preferences, false).Value;
    }

    /// <summary>
    /// Asynchronously gets an HTTP connection to an innovator instance (or proxy) at the given URL
    /// </summary>
    /// <param name="url">URL of the innovator instance (or proxy)</param>
    /// <param name="preferences">Object containing preferences for the connection</param>
    /// <param name="async">Whether or not to return the connection asynchronously.  This is important
    /// as an HTTP request must be issued to determine the type of connection to create</param>
    /// <returns>A promise to return a connection object</returns>
    public static IPromise<IRemoteConnection> GetConnection(string url
      , ConnectionPreferences preferences, bool async)
    {
      preferences = preferences ?? new ConnectionPreferences();

      url = (url ?? "").TrimEnd('/');
      if (url.EndsWith("Server/InnovatorServer.aspx", StringComparison.OrdinalIgnoreCase))
        url = url.Substring(0, url.Length - 21);
      if (!url.EndsWith("/server", StringComparison.OrdinalIgnoreCase)) url += "/Server";
      var configUrl = url + "/mapping.xml";

      var masterService = preferences.HttpService ?? DefaultService.Invoke();
      var arasSerice = preferences.HttpService ?? new DefaultHttpService() { Compression = CompressionType.none };
      Func<ServerMapping, IRemoteConnection> connFactory = m =>
      {
        var uri = (m.Url ?? "").TrimEnd('/');
        if (!uri.EndsWith("/server", StringComparison.OrdinalIgnoreCase)) url += "/Server";
        switch (m.Type)
        {
          case ServerType.Proxy:
            m.Endpoints.Base = new Uri(uri + "/");
            var conn = new Connection.ProxyServerConnection(masterService, m.Endpoints, preferences.Deserializer);
            conn.SessionPolicy = preferences.SessionPolicy;
            if (!string.IsNullOrEmpty(preferences.UserAgent))
              conn.DefaultSettings(req => req.UserAgent = preferences.UserAgent);
            return conn;
          default:
            return ArasConn(arasSerice, uri, preferences);
        }
      };

      var result = new Promise<IRemoteConnection>();
      result.CancelTarget(masterService.Execute("GET", configUrl, null, CredentialCache.DefaultCredentials
                                          , async, request =>
        {
          request.UserAgent = preferences.UserAgent;
          request.SetHeader("Accept", "text/xml");
        }).Progress((p, m) => result.Notify(p, m))
        .Done(r => {
          var data = r.AsString();
          if (string.IsNullOrEmpty(data))
          {
            result.Resolve(ArasConn(arasSerice, url, preferences));
          }
          else
          {
            try
            {
              var servers = ServerMapping.FromXml(data).ToArray();
              if (servers.Length < 1)
              {
                result.Resolve(ArasConn(arasSerice, url, preferences));
              }
              else if (servers.Length == 1)
              {
                result.Resolve(connFactory(servers.Single()));
              }
              else
              {
                foreach (var server in servers)
                {
                  server.Factory = connFactory;
                }
                result.Resolve(new MappedConnection(servers, preferences.AllowAuthPreCheck));
              }
            }
            catch (XmlException)
            {
              result.Resolve(ArasConn(arasSerice, url, preferences));
            }
          }
        }).Fail(ex => {
          result.Resolve(ArasConn(arasSerice, url, preferences));
        }));
      return result;
    }

    private static IRemoteConnection ArasConn(IHttpService arasService, string url, ConnectionPreferences preferences)
    {
      var result = new Connection.ArasHttpConnection(arasService, url, preferences.Deserializer);
      if (!string.IsNullOrEmpty(preferences.Locale)
        || !string.IsNullOrEmpty(preferences.TimeZone)
        || !string.IsNullOrEmpty(preferences.UserAgent))
      {
        result.DefaultSettings(r => {
          if (!string.IsNullOrEmpty(preferences.Locale))
            r.SetHeader("LOCALE", preferences.Locale);
          if (!string.IsNullOrEmpty(preferences.TimeZone))
            r.SetHeader("TIMEZONE_NAME", preferences.TimeZone);
          if (!string.IsNullOrEmpty(preferences.UserAgent))
            r.UserAgent = preferences.UserAgent;
        });
      }
      return result;
    }
  }
}
