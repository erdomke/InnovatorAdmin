using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Innovator.Client.Connection
{
  public class ArasHttpConnection : IRemoteConnection, IArasConnection
  {
    private IHttpService _service;
    private int _arasVersion;
    private ServerContext _context = new ServerContext();
    private ElementFactory _factory;
    private string _httpDatabase;
    private string _httpPassword;
    private string _httpUsername;
    private ICredentials _lastCredentials;
    private Uri _innovatorServerBaseUrl;
    private Uri _innovatorServerUrl;
    private Uri _innovatorClientBin;
    private string _userId;
    private List<Action<IHttpRequest>> _defaults = new List<Action<IHttpRequest>>();
    private ArasVaultConnection _vaultConn;
    private List<KeyValuePair<string, string>> _serverInfo = new List<KeyValuePair<string, string>>();

    public ElementFactory AmlContext
    {
      get { return _factory; }
    }

    public CompressionType Compression
    {
      get { return _service.Compression; }
      set { _service.Compression = value; }
    }
    public string Database
    {
      get { return _httpDatabase; }
    }
    public IEnumerable<KeyValuePair<string, string>> ServerInfo
    {
      get { return _serverInfo; }
    }
    public Uri Url
    {
      get { return _innovatorServerBaseUrl; }
    }
    public string UserId
    {
      get { return _userId; }
    }
    public int Version
    {
      get { return _arasVersion; }
    }

    public ArasHttpConnection(IHttpService service, string innovatorServerUrl)
    {
      _service = service;
      this.Compression = CompressionType.none;
      _factory = new ElementFactory(_context);

      if (innovatorServerUrl.EndsWith("Server/InnovatorServer.aspx", StringComparison.OrdinalIgnoreCase))
      {
        innovatorServerUrl = innovatorServerUrl.Substring(0, innovatorServerUrl.Length - 20);
      }
      else if (innovatorServerUrl.EndsWith("/Server", StringComparison.OrdinalIgnoreCase)
        || innovatorServerUrl.EndsWith("/Server/", StringComparison.OrdinalIgnoreCase))
      {
        innovatorServerUrl += (innovatorServerUrl.EndsWith("/") ? "" : "/");
      }
      else
      {
        innovatorServerUrl += (innovatorServerUrl.EndsWith("/") ? "" : "/") + "Server/";
      }

      this._innovatorServerBaseUrl = new Uri(innovatorServerUrl);
      this._innovatorServerUrl = new Uri(this._innovatorServerBaseUrl, "InnovatorServer.aspx");
      this._innovatorClientBin = new Uri(this._innovatorServerBaseUrl, "../Client/cbin/");

      _vaultConn = new ArasVaultConnection(this);
    }
    /// <summary>
    /// Process a command by crafting the appropriate HTTP request and returning the HTTP response stream
    /// </summary>
    public Stream Process(Command request)
    {
      var upload = request as UploadCommand;
      if (upload == null)
      {
        if (request.Action == CommandAction.DownloadFile)
          return _vaultConn.Download(request, false).Value;

        return UploadAml(_innovatorServerUrl, request.Action.ToString(), request, false).Value.AsStream;
      }
      return Process(request, false).Value;
    }
    /// <summary>
    /// Process a command asynchronously by crafting the appropriate HTTP request and returning the HTTP response stream
    /// </summary>
    public IPromise<Stream> Process(Command request, bool async)
    {
      var upload = request as UploadCommand;
      if (upload == null)
      {
        if (request.Action == CommandAction.DownloadFile)
          return _vaultConn.Download(request, async);

        return UploadAml(_innovatorServerUrl, request.Action.ToString(), request, async)
          .Convert(r => r.AsStream);
      }
      else if (request.Action == CommandAction.DownloadFile)
      {
        throw new ArgumentException("Cannot download a file with an upload request.");
      }

      // Files need to be uploaded, so build the vault request
      return _vaultConn.Upload(upload, async);
    }

    public UploadCommand CreateUploadCommand()
    {
      return new UploadCommand(_vaultConn.VaultStrategy.WritePriority(false).Value.First());
    }

    public IEnumerable<string> GetDatabases()
    {
      var req = GetBasicRequest(new Uri(this._innovatorServerBaseUrl, "DBList.aspx"));
      using (var reader = XmlReader.Create(req.Execute().AsStream))
      {
        while (reader.Read())
        {
          if (reader.NodeType == XmlNodeType.Element && reader.LocalName == "DB"
            && !string.IsNullOrEmpty(reader.GetAttribute("id")))
          {
            yield return reader.GetAttribute("id");
          }
        }
      }
    }

    public void Login(ICredentials credentials)
    {
      // Access the value property to force throwing any appropriate exception
      var result = Login(credentials, false).Value;
    }

    public IPromise<string> Login(ICredentials credentials, bool async)
    {
      var explicitCred = credentials as ExplicitCredentials;
      if (explicitCred == null) throw new NotSupportedException("This connection implementation only supports explicit credentials");

      _httpDatabase = explicitCred.Database;
      _httpUsername = explicitCred.Username;
      _httpPassword = explicitCred.Password == null ? _httpPassword
        : explicitCred.Password.UseBytes<string>((ref byte[] b) =>
                                                  CalcMd5(ref b).ToLowerInvariant());
      _lastCredentials = credentials;

      var result = new Promise<string>();
      result.CancelTarget(
        Process(new Command("<Item/>").WithAction(CommandAction.ValidateUser), async)
          .Progress((p, m) => result.Notify(p, m))
          .Done(r =>
          {
            string xml;
            using (var reader = new StreamReader(r))
            {
              xml = reader.ReadToEnd();
            }

            var data = XElement.Parse(xml).DescendantsAndSelf("Result").FirstOrDefault();
            if (data == null)
            {
              var res = ElementFactory.Local.FromXml(xml);
              var ex = res.Exception ?? ElementFactory.Local.ServerException("Failed to login");
              ex.SetDetails(_httpDatabase, "<Item/>");

              _httpDatabase = null;
              _httpUsername = null;
              _httpPassword = null;
              result.Reject(ex);
            }
            else
            {
              foreach (var elem in data.Elements())
              {
                switch (elem.Name.LocalName)
                {
                  case "id":
                    _userId = elem.Value;
                    break;
                  case "i18nsessioncontext":
                    var context = new ServerContext();
                    _context.DefaultLanguageCode = elem.Element("default_language_code").Value;
                    _context.DefaultLanguageSuffix = elem.Element("default_language_suffix").Value;
                    _context.LanguageCode = elem.Element("language_code").Value;
                    _context.LanguageSuffix = elem.Element("language_suffix").Value;
                    _context.Locale = elem.Element("locale").Value;
                    _context.TimeZone = elem.Element("time_zone").Value;
                    break;
                  case "ServerInfo":
                    foreach (var info in elem.Elements())
                    {
                      if (info.Name.LocalName == "Version")
                        _arasVersion = int.Parse(info.Value.Substring(0, info.Value.IndexOf('.')));

                      if (!string.IsNullOrEmpty(elem.Value))
                        _serverInfo.Add(new KeyValuePair<string, string>("ServerInfo/" + elem.Name.LocalName, elem.Value));
                    }
                    break;
                  default:
                    if (!string.IsNullOrEmpty(elem.Value))
                      _serverInfo.Add(new KeyValuePair<string, string>(elem.Name.LocalName, elem.Value));
                    break;
                }
              }

              _vaultConn.VaultStrategy.Initialize(this);
              result.Resolve(_userId);
            }
          }).Fail(ex =>
          {
            _httpDatabase = null;
            _httpUsername = null;
            _httpPassword = null;
            result.Reject(ex);
          }));
      return result;
    }

    public void LoginToken(string database, string username, SecureToken token)
    {
      _httpPassword = token.UseString<string>((ref string p) => new string(p.ToCharArray()));
      Login(new ExplicitCredentials(database, username, null), false);
    }

    public void Logout(bool unlockOnLogout)
    {
      Logout(unlockOnLogout, false);
    }

    public void Logout(bool unlockOnLogout, bool async)
    {
      Process(new Command("<logoff skip_unlock=\"" + (unlockOnLogout ? 0 : 1) + "\"/>").WithAction(CommandAction.LogOff), async)
        .Done(r =>
        {
          _context = null;
          _factory = null;
          _httpDatabase = null;
          _httpPassword = null;
          _httpUsername = null;
          _userId = null;
        });
    }

    public void DefaultSettings(Action<IHttpRequest> callback)
    {
      _defaults.Add(callback);
    }

    public void SetVaultStrategy(IVaultStrategy strategy)
    {
      _vaultConn.VaultStrategy = strategy;
      if (!string.IsNullOrEmpty(_userId)) _vaultConn.VaultStrategy.Initialize(this);
    }

    private IPromise<IHttpResponse> UploadAml(Uri uri, string action, Command request, bool async)
    {
      return _service.Execute("POST", uri.ToString(), null, CredentialCache.DefaultCredentials, async, req =>
      {
        ((IArasConnection)this).SetDefaultHeaders((k, v) => { req.SetHeader(k, v); });

        foreach (var a in _defaults)
        {
          a.Invoke(req);
        }
        if (request.Settings != null) request.Settings.Invoke(req);

        if (!string.IsNullOrEmpty(action)) req.SetHeader("SOAPACTION", action);
        req.SetContent(w => w.Write("<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + request.ToNormalizedAml(_factory.LocalizationContext)).Close(), "text/xml");
      });
    }

    private WebRequest GetBasicRequest(Uri uri)
    {
      var req = (HttpWebRequest)System.Net.WebRequest.Create(uri);
      req.Credentials = CredentialCache.DefaultCredentials;
      req.Proxy.Credentials = CredentialCache.DefaultCredentials;

      var result = new WebRequest(req, this.Compression);
      foreach (var a in _defaults)
      {
        a.Invoke(result);
      }
      return result;
    }

    internal IPromise<string> GetResult(string action, string request, bool async)
    {
      var result = new Promise<string>();
      result.CancelTarget(
        UploadAml(_innovatorServerUrl, "ApplyItem", request, async)
          .Progress((p, m) => result.Notify(p, m))
          .Done(r =>
          {
            var res = _factory.FromXml(r.AsString(), request, this);
            if (res.Exception == null)
            {
              result.Resolve(res.Value);
            }
            else
            {
              result.Reject(res.Exception);
            }
          }).Fail(ex => { result.Reject(ex); }));
      return result;
    }

    void IArasConnection.SetDefaultHeaders(Action<string, string> writer)
    {
      writer.Invoke("AUTHUSER", this._httpUsername);
      writer.Invoke("AUTHPASSWORD", this._httpPassword);
      writer.Invoke("DATABASE", this._httpDatabase);
      writer.Invoke("LOCALE", this._context.Locale);
      writer.Invoke("TIMEZONE_NAME", this._context.TimeZone);
    }

    public void Dispose()
    {
      if (!string.IsNullOrEmpty(_userId)) Logout(true);
    }

    public string MapClientUrl(string relativeUrl)
    {
      return new Uri(this._innovatorClientBin, relativeUrl).ToString();
    }

    List<Action<IHttpRequest>> IArasConnection.DefaultSettings
    {
      get { return _defaults; }
    }

    private string CalcMd5(ref byte[] value)
    {
      using (var md5Provider = new MD5CryptoServiceProvider())
      {
        return md5Provider.ComputeHash(value).HexString();
      }
    }

    public override bool Equals(object obj)
    {
      var conn = obj as ArasHttpConnection;
      if (conn == null) return false;
      return Equals(conn);
    }
    public bool Equals(ArasHttpConnection conn)
    {
      return conn._innovatorServerBaseUrl.Equals(this._innovatorServerBaseUrl)
        && String.Equals(conn._httpDatabase, this._httpDatabase)
        && String.Equals(conn._userId, this._userId);
    }
    public override int GetHashCode()
    {
      return this._innovatorServerBaseUrl.GetHashCode()
        ^ (_httpDatabase ?? "").GetHashCode()
        ^ (_userId ?? "").GetHashCode();
    }

    public IPromise<IRemoteConnection> Clone(bool async)
    {
      var newConn = new ArasHttpConnection(new DefaultHttpService() { Compression = CompressionType.none }, _innovatorServerUrl.ToString());
      newConn._defaults = this._defaults;
      return newConn.Login(_lastCredentials, async)
        .Convert(u => (IRemoteConnection)newConn);
    }
  }
}
