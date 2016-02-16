using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using Innovator.Client;
using System.Net;

namespace Innovator.Client.Connection
{
  public class ProxyServerConnection : IRemoteConnection
  {
    public const string PolicyTokenHeader = "Policy-Token";

    private string[] _databases;
    private string _database;
    private Action<IHttpRequest> _defaults = r => { };
    private Endpoints _endpoints;
    private ElementFactory _factory;
    private IHttpService _service;
    private InitializeSessionToken _lastLoginToken;
    private TokenCredentials _renewalToken;
    private string _sessionToken;
    private string _userId;
    private System.Timers.Timer _timer;
    private IPromise<Vault> _writeVault;

    public ElementFactory AmlContext
    {
      get { return _factory; }
    }
    public string Database
    {
      get { return _database; }
    }
    public SessionPolicy SessionPolicy { get; set; }
    public Uri Url
    {
      get
      {
        return _endpoints.Base;
      }
    }
    public string UserId
    {
      get { return _userId; }
    }

    internal ProxyServerConnection(IHttpService service, Endpoints endpoints)
    {
      _service = service;
      _endpoints = endpoints;
      _timer = new System.Timers.Timer();
      _timer.Elapsed += _timer_Elapsed;
    }

    void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      _timer.Enabled = false;
      PolicyToken(PolicyTokenType.connection, null, null, true)
        .Continue(p =>
        {
          return RenewSession(_renewalToken.Content, p, true).Done(r =>
          {
            var data = r.AsXml();
            var auth = data.Descendants("Authorization").First();
            var elem = auth.Element("access_token");
            if (elem != null)
              SetSessionToken(elem.Value,
                int.Parse(auth.Element("expires_in").Value));
          });
        });
    }

    private void SetSessionToken(string value, int interval)
    {
      _sessionToken = value;
      // Get a new token 2 seconds before the current one expires
      _timer.Interval = (interval - 2) * 1000;
      _timer.Enabled = true;
    }

    public Stream Process(Command request)
    {
      return Process(request, false).Value;
    }
    public IPromise<Stream> Process(Command request, bool async)
    {
      var upload = request as UploadCommand;
      if (upload == null)
      {
        if (request.Action == CommandAction.DownloadFile)
          return DownloadFile(request, async);

        return Query(_sessionToken, request.Action.ToString(), request.ToNormalizedAml(_factory.LocalizationContext)
                    , this.SessionPolicy, null, async)
          .Convert(r => r.AsStream);
      } else if (request.Action == CommandAction.DownloadFile)
      {
        throw new ArgumentException("Cannot download a file with an upload request.");
      }

      var multiWriter = new MultiPartFormWriter(async, _factory.LocalizationContext);
      multiWriter.AddFiles(upload);
      multiWriter.WriteFormField("SOAPACTION", request.Action.ToString());
      multiWriter.WriteFormField("XMLdata", "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:i18n=\"http://www.aras.com/I18N\"><SOAP-ENV:Body><ApplyItem>"
        + upload.ToNormalizedAml(_factory.LocalizationContext)
        + "</ApplyItem></SOAP-ENV:Body></SOAP-ENV:Envelope>");

      return _service.Execute("POST", new Uri(_endpoints.Base, _endpoints.Query.First()).ToString()
        , null, CredentialCache.DefaultCredentials, async, req =>
      {
        req.ConfigureForFileUpload();
        _defaults.Invoke(req);
        req.Timeout = -1;
        req.SetHeader("Content-Length", multiWriter.GetLength().ToString());
        req.SetHeader("Accept", "text/xml");
        if (!string.IsNullOrEmpty(_sessionToken))
          req.SetHeader("Authorization", "Bearer " + _sessionToken);
        req.SetHeader("SOAPACTION", request.Action.ToString());
        req.SetContent(multiWriter.WriteToRequest, multiWriter.ContentType);
      }).Convert(r => r.AsStream);

    }

    public IEnumerable<string> GetDatabases()
    {
      if (_databases == null)
      {
        _databases = Databases(false).Value.AsXml().Elements("DB")
          .Select(e => e.Attribute("id").Value).ToArray();
      }
      return _databases;
    }

    public void Login(ICredentials credentials)
    {
      var value = Login(credentials, false).Value;
    }

    public IPromise<string> Login(ICredentials credentials, bool async)
    {
      var result = new Promise<string>();

      var database = string.Empty;
      var tokenCred = credentials as TokenCredentials;
      IPromise<IHttpResponse> loginPromise;

      if (tokenCred != null)
      {
        database = tokenCred.Database;
        loginPromise = PolicyToken(PolicyTokenType.connection, null, null, true)
          .Continue(p =>
          {
            return RenewSession(tokenCred.Content, p, true);
          });
      }
      else
      {
        var tokenPromise = (_lastLoginToken == null
                        || _lastLoginToken.Expiration > DateTime.UtcNow.AddSeconds(-5)) ?
        Query(null, null, null, this.SessionPolicy, null, async)
          .Convert<IHttpResponse, InitializeSessionToken>((r, p) =>
          {
            p.Reject(new Exception("Unauthorized error expected"));
          }, (ex, p) =>
          {
            var httpEx = ex as HttpException;
            if (httpEx != null
              && httpEx.Response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
              var header = AuthenticationScheme.Parse(httpEx.Response.Headers["WWW-Authenticate"]);
              var auth = header.FirstOrDefault(a => a.Name == "bearer");
              if (auth == null) throw new InvalidOperationException();
              var ssoAuth = header.FirstOrDefault(a => a.Name == "winsso");

              _lastLoginToken = new InitializeSessionToken(auth.Parameters["token"]
                                      , auth.Parameters["nonce"], auth.Parameters["public_key"]);
              _lastLoginToken.SsoUrl = ssoAuth.Parameters["uri"];
              p.Resolve(_lastLoginToken);
            }
            else
            {
              p.Reject(ex);
            }
          }) :
        Promises.Resolved(_lastLoginToken);

      loginPromise = Promises.All(tokenPromise
                              , PolicyToken(PolicyTokenType.connection, null, null, async))
        .Continue(r =>
        {
          var winCred = credentials as WindowsCredentials;
          if (winCred == null)
          {
            SecureToken password = null;
            var username = string.Empty;

            var explicitCred = credentials as ExplicitCredentials;
            if (explicitCred != null)
            {
              database = explicitCred.Database;
              password = explicitCred.Password;
              username = explicitCred.Username;
            }
            else
            {
              var anon = credentials as AnonymousCredentials;
              if (anon != null)
              {
                database = anon.Database;
              }
              else
              {
                throw new ArgumentException(string.Format("Login credentials must be one of the built-in types, {0} is not supported"
                  , credentials == null, "NULL", credentials.GetType()), "credentials");
              }
            }

            string encodedData;
            var usernameLength = (username == null ? 0 : username.Length);
            var passwordLength = (password == null ? 0 : password.Length);
            var buffer = new byte[3 + 2 * (r.Result1.Nonce.Length + database.Length
                                            + usernameLength + passwordLength)];
            try
            {
              var i = Encoding.UTF8.GetBytes(r.Result1.Nonce, 0, r.Result1.Nonce.Length, buffer, 0);
              buffer[i++] = (byte)'|';
              i += Encoding.UTF8.GetBytes(database, 0, database.Length, buffer, i);
              buffer[i++] = (byte)'|';
              if (usernameLength > 0)
                i += Encoding.UTF8.GetBytes(username, 0, username.Length, buffer, i);
              buffer[i++] = (byte)'|';
              if (passwordLength > 0) password.UseBytes<bool>((ref byte[] b) =>
              {
                for (var j = 0; j < b.Length; j++)
                {
                  buffer[j + i] = b[j];
                }
                i += b.Length;
                return false;
              });

              encodedData = Convert.ToBase64String(r.Result1.Encryptor.Encrypt(buffer, 0, i));
            }
            finally
            {
              for (var j = 0; j < buffer.Length; j++)
              {
                buffer[j] = 0;
              }
            }
            return Query(r.Result1.Content + " " + encodedData, "ValidateUser", "<Item/>"
              , this.SessionPolicy , r.Result2, async);
          }
          else
          {
            // Windows authentication
            return Query(r.Result1.SsoUrl, r.Result1.Content, "ValidateUser", "<Item/>"
              , this.SessionPolicy, r.Result2, winCred.Credentials, async
              , req => req.SetHeader("DATABASE", winCred.Database));
          }
        });
      }

      loginPromise.Progress((p, m) => result.Notify(p, m))
        .Done(r => {
          _database = database;

          var data = r.AsXml().DescendantsAndSelf("Result").FirstOrDefault();
          _userId = data.Element("id").Value;

          var auth = data.Element("Authorization");
          _renewalToken = new TokenCredentials(auth.Element("refresh_token").Value);
          _renewalToken.Database = database;
          SetSessionToken(auth.Element("access_token").Value
            , int.Parse(auth.Element("expires_in").Value));

          var i18n = data.Element("i18nsessioncontext");
          var context = new ServerContext();
          context.DefaultLanguageCode = i18n.Element("default_language_code").Value;
          context.DefaultLanguageSuffix = i18n.Element("default_language_suffix").Value;
          context.LanguageCode = i18n.Element("language_code").Value;
          context.LanguageSuffix = i18n.Element("language_suffix").Value;
          context.Locale = i18n.Element("locale").Value;
          context.TimeZone = i18n.Element("time_zone").Value;
          _factory = new ElementFactory(context);

          var upload = data.Element("WriteVault") == null
            ? null : data.Element("WriteVault").Element("Item");
          if (upload == null)
          {
            var strategy = new DefaultVaultStrategy();
            strategy.Initialize(this);
            _writeVault = strategy.WritePriority(true).Convert(v => v.First());
          }
          else
          {
            _writeVault = Promises.Resolved(Vault.GetVault(
              (IReadOnlyItem)_factory.FromXml(upload.ToString())));
          }

          result.Resolve(_userId);
        }).Fail(ex => {
          var httpEx = ex as HttpException;
          if (httpEx != null
            && httpEx.Response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
          {
            var auth = AuthenticationScheme.Parse(httpEx.Response.Headers["WWW-Authenticate"])
              .FirstOrDefault(a => a.Name == "bearer");
            string msg;
            if (auth != null && auth.Parameters.TryGetValue("error_description", out msg))
            {
              result.Reject(new Exception("Error logging in: " + msg));
            }
            else
            {
              result.Reject(new Exception("Unanticipated error logging in."));
            }
          }
          else
          {
            result.Reject(ex);
          }
        });
      result.CancelTarget(loginPromise);

      return result;
    }

    private IPromise<Stream> DownloadFile(Command request, bool async)
    {
      var parsedAml = _factory.FromXml(request.ToNormalizedAml(_factory.LocalizationContext));
      var file = parsedAml.AssertItem("File");
      if (string.IsNullOrEmpty(file.Id()))
        return Promises.Rejected<Stream>(new ArgumentException("Request does not represent a single file item", "request"));
      var downloadBase = new Uri(_endpoints.Base, _endpoints.Download.First());
      return _service.Execute("GET"
        , new Uri(downloadBase, Uri.EscapeUriString(_database) + "/"
                                + Uri.EscapeUriString(file.Id())).ToString()
        , null, CredentialCache.DefaultCredentials, async, _defaults)
        .Convert(r => r.AsStream);
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
          _database = null;
          _factory = null;
          _renewalToken = null;
          _sessionToken = null;
          _userId = null;
          _timer.Enabled = false;
          _writeVault = null;
        });
    }

    public UploadCommand CreateUploadCommand()
    {
      return new UploadCommand(_writeVault.Wait());
    }

    public void DefaultSettings(Action<IHttpRequest> callback)
    {
      _defaults = callback;
    }

    private enum PolicyTokenType
    {
      connection,
      privateDevice
    }

    private IPromise<string> PolicyToken(PolicyTokenType type, string userId
                                        , string database, bool async)
    {
      var result = new Promise<string>();
      result.CancelTarget(PolicyService(type.ToString(), userId, database, async)
        .Progress((p, m) => result.Notify(p, m))
        .Done(r => {
          result.Resolve(r.AsString());
        })
        .Fail(ex =>
        {
          if (ex is NullReferenceException || ex is HttpException)
          {
            result.Resolve(string.Empty);
          }
          else
          {
            result.Reject(ex);
          }
        }));
      return result;
    }

    private IPromise<IHttpResponse> Databases(bool async)
    {
      return _service.Execute("GET"
        , new Uri(_endpoints.Base, _endpoints.Databases.First()).ToString()
        , null, CredentialCache.DefaultCredentials, async, request =>
      {
        _defaults.Invoke(request);
        request.SetHeader("Accept", "text/xml");
      });
    }
    private IPromise<IHttpResponse> Query(string authorization, string action, string query,
      SessionPolicy policy, string policyToken, bool async)
    {
      return Query(_endpoints.Query.First(), authorization, action, query, policy, policyToken
                  , CredentialCache.DefaultCredentials, async);
    }
    private IPromise<IHttpResponse> Query(string queryEndpoint, string authorization
      , string action, string query, SessionPolicy policy, string policyToken
      , System.Net.ICredentials credentials, bool async, Action<IHttpRequest> writer = null)
    {
      return _service.Execute("POST", new Uri(_endpoints.Base, queryEndpoint).ToString()
                              , null, credentials, async, request =>
      {
        _defaults.Invoke(request);
        request.SetHeader("Accept", "text/xml");
        if (action == "ValidateUser")
        {
          request.SetHeader("DesiredPolicy", ((int)policy).ToString());
          if (!string.IsNullOrEmpty(policyToken))
            request.SetHeader(ProxyServerConnection.PolicyTokenHeader, policyToken);
        }
        if (!string.IsNullOrEmpty(authorization))
          request.SetHeader("Authorization", "Bearer " + authorization);
        if (!string.IsNullOrEmpty(action)) request.SetHeader("SOAPACTION", action);
        if (writer != null) writer.Invoke(request);
        if (!string.IsNullOrEmpty(query)) request.SetContent(w => w.Write(query).Close(), "text/xml");
      });
    }
    private IPromise<IHttpResponse> RenewSession(string renewalToken, string policyToken, bool async)
    {
      return _service.Execute("POST", new Uri(_endpoints.Base, _endpoints.RenewSession.First()).ToString()
        , null, CredentialCache.DefaultCredentials, async, request =>
      {
        _defaults.Invoke(request);
        request.SetHeader("Accept", "text/xml");
        if (!string.IsNullOrEmpty(policyToken))
          request.SetHeader(ProxyServerConnection.PolicyTokenHeader, policyToken);
        if (!string.IsNullOrEmpty(renewalToken))
          request.SetHeader("Authorization", "Bearer " + renewalToken);
      });
    }
    public IPromise<IHttpResponse> PolicyService(string type, string userId
                                                , string database, bool async)
    {
      if (!_endpoints.PolicyService.Any())
        return Promises.Rejected<IHttpResponse>(new NullReferenceException("No service url defined"));

      var query = new QueryString() { {"type", type} };
      if (!string.IsNullOrEmpty(userId)) query.Add("userid", userId);
      if (!string.IsNullOrEmpty(userId)) query.Add("database", database);

      return _service.Execute("GET"
        , new Uri(_endpoints.Base, _endpoints.PolicyService.First()).ToString()
        , query, CredentialCache.DefaultCredentials, async, request =>
      {
        _defaults.Invoke(request);
        request.SetHeader("Accept", "text/xml");
      });
    }

    public void Dispose()
    {
      if (!string.IsNullOrEmpty(_userId)) Logout(true);
    }


    public string MapClientUrl(string relativeUrl)
    {
      throw new NotImplementedException();
    }

    public override bool Equals(object obj)
    {
      var conn = obj as ProxyServerConnection;
      if (conn == null) return false;
      return Equals(conn);
    }
    public bool Equals(ProxyServerConnection conn)
    {
      return conn._endpoints.Base.Equals(this._endpoints.Base)
        && String.Equals(conn._database, this._database)
        && String.Equals(conn._userId, this._userId);
    }
    public override int GetHashCode()
    {
      return this._endpoints.Base.GetHashCode()
        ^ (_database ?? "").GetHashCode()
        ^ (_userId ?? "").GetHashCode();
    }
  }
}
