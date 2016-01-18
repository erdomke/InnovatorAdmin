using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Innovator.Client
{
  internal class WebRequest : IHttpRequest
  {
    private HttpWebRequest _request;
    private Action<IStreamWriter> _requestWriter;
    private CompressionType _compression;

    public CompressionType Compression
    {
      get { return _compression; }
    }
    public long ContentLength
    {
      get { return _request.ContentLength; }
      internal set { _request.ContentLength = value; }
    }
    public bool PreAuthenticate
    {
      get { return _request.PreAuthenticate; }
      set { _request.PreAuthenticate = value; }
    }
    public int ReadWriteTimeout
    {
      get { return _request.ReadWriteTimeout; }
      set { _request.ReadWriteTimeout = value; }
    }
    public int Timeout
    {
      get { return _request.Timeout; }
      set { _request.Timeout = value; }
    }
    public bool UnsafeAuthenticatedConnectionSharing
    {
      get { return _request.UnsafeAuthenticatedConnectionSharing; }
      set { _request.UnsafeAuthenticatedConnectionSharing = value; }
    }
    public string UserAgent
    {
      get { return _request.UserAgent; }
      set { _request.UserAgent = value; }
    }
    internal HttpWebRequest Core
    {
      get { return _request; }
    }

    public WebRequest(HttpWebRequest request, CompressionType compression)
    {
      _request = request;
      _compression = compression;
    }

    public WebResponse Execute()
    {
      if (_requestWriter != null && _request.Method != "GET")
      {
        var stream = new SmartCompressionStream(_compression, _request.ContentType);
        var writer = new SyncStreamWriter(stream);
        _requestWriter.Invoke(writer);
        writer.Close();

        var memStream = stream.BaseStream;
        _request.ContentLength = memStream.Position;
        if (stream.Compression != CompressionType.none)
        {
          _request.Headers.Add("Content-Encoding", stream.Compression.ToString());
        }

        memStream.Position = 0;
        using (var reqStream = _request.GetRequestStream())
        {
          memStream.CopyTo(reqStream);
        }
      }
      else if (_request.Method == "POST")
      {
        _request.ContentLength = 0;
      }
      return new WebResponse(_request, (HttpWebResponse)_request.GetResponse());
    }
    [System.Diagnostics.DebuggerStepThrough()]
    public WebPromise Execute(bool async)
    {
      var promise = new WebPromise(this);
      if (async)
      {
        promise.Execute(_requestWriter);
      }
      else
      {
        try
        {
          promise.Resolve(Execute());
        }
        catch (WebException webEx)
        {
          switch (webEx.Status)
          {
            case WebExceptionStatus.RequestCanceled:
              promise.Cancel();
              break;
            case WebExceptionStatus.Timeout:
              promise.Reject(new HttpTimeoutException());
              break;
            default:
              promise.Reject(new HttpException(webEx, _request));
              break;
          }
        }
        catch (Exception ex)
        {
          promise.Reject(ex);
        }
      }
      return promise;
    }

    public void SetContent(string content)
    {
      _requestWriter = (s) =>
      {
        s.Write(content).Close();
      };
    }
    public void SetContent(Action<IStreamWriter> writer, string contentType = null)
    {
      if (!string.IsNullOrEmpty(contentType)) _request.ContentType = contentType;
      _requestWriter = writer;
    }
    public void SetHeader(string name, string value)
    {
      switch (name.ToLowerInvariant())
      {
        case "accept":
          _request.Accept = value;
          break;
        case "connection":
          if (value.Equals("keep-alive", StringComparison.OrdinalIgnoreCase))
          {
            _request.KeepAlive = true;
          }
          else
          {
            _request.Connection = value;
          }
          break;
        case "content-length":
          _request.ContentLength = long.Parse(value);
          break;
        case "content-type":
          _request.ContentType = value;
          break;
        case "expect":
          _request.Expect = value;
          break;
        case "host":
#if NET4
          _request.Host = value;
#endif
          break;
        case "if-modified-since":
          _request.IfModifiedSince = DateTime.Parse(value);
          break;
        case "media-type":
          _request.MediaType = value;
          break;
        case "referer":
          _request.Referer = value;
          break;
        case "transfer-encoding":
          _request.TransferEncoding = value;
          break;
        case "user-agent":
          _request.UserAgent = value;
          break;
        default:
          if (value == null)
          {
            _request.Headers.Remove(name);
          }
          else
          {
            _request.Headers.Set(name, value);
          }
          break;
      }
    }

    public AuthenticationSchemes CheckForNotAuthorized(IHttpResponse resp)
    {
      string authenticate;
      if (!resp.Headers.TryGetValue("WWW-Authenticate", out authenticate)) return AuthenticationSchemes.Anonymous;

      if (authenticate.IndexOf("Negotiate", StringComparison.OrdinalIgnoreCase) == 0)
      {
        try
        {
          AuthenticationManager.Authenticate("Negotiate", _request, _request.Credentials ?? CredentialCache.DefaultNetworkCredentials);
          var authStateProp = typeof(HttpWebRequest).GetProperty("CurrentAuthenticationState", BindingFlags.Instance | BindingFlags.NonPublic);
          var authState = authStateProp.GetValue(_request, null);
          var secContextField = authState.GetType().GetField("SecurityContext", BindingFlags.Instance | BindingFlags.NonPublic);
          var secContext = secContextField.GetValue(authState);
          var IsKerberosProp = secContext.GetType().GetProperty("IsKerberos", BindingFlags.Instance | BindingFlags.NonPublic);
          if ((bool)IsKerberosProp.GetValue(secContext, null))
          {
            return AuthenticationSchemes.Negotiate;
          }
        }
        catch {}
        return AuthenticationSchemes.Ntlm;
      }
      else if (authenticate.IndexOf("NTLM", StringComparison.OrdinalIgnoreCase) == 0)
      {
        return AuthenticationSchemes.Ntlm;
      }
      else if (authenticate.IndexOf("Basic", StringComparison.OrdinalIgnoreCase) == 0)
      {
        return AuthenticationSchemes.Basic;
      }
      return AuthenticationSchemes.None;
    }


    public void ConfigureForFileUpload()
    {
      _request.AllowWriteStreamBuffering = false;
      _request.SendChunked = true;
      _request.KeepAlive = true;
      _request.ProtocolVersion = HttpVersion.Version11;
    }
  }
}
