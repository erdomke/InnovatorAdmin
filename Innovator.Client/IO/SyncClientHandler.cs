using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Innovator.Client
{
  internal class SyncClientHandler : HttpClientHandler
  {
#if HTTPSYNC
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var req = request as HttpRequest;
      if (req == null || req.Async || (req.Content != null && !(req.Content is ISyncContent)))
        return base.SendAsync(request, cancellationToken);

      var factory = new TaskCompletionSource<HttpResponseMessage>();
      try
      {
        factory.SetResult(Send(req));
      }
      catch (HttpTimeoutException)
      {
        factory.SetCanceled();
      }
      catch (Exception ex)
      {
        factory.SetException(ex);
      }

      return factory.Task;
    }

    public HttpResponseMsg Send(HttpRequest request)
    {
      var wReq = CreateAndPrepareWebRequest(request);
      wReq.Timeout = request.Timeout;
      var syncContent = request.Content as ISyncContent;
      if (syncContent != null)
      {
        syncContent.SerializeToStream(wReq.GetRequestStream());
      }

      try
      {
        return CreateResponseMessage((HttpWebResponse)wReq.GetResponse(), request);
      }
      catch (WebException webex)
      {
        switch (webex.Status)
        {
          case WebExceptionStatus.RequestCanceled:
          case WebExceptionStatus.Timeout:
            throw new HttpTimeoutException(string.Format("A response was not received after waiting for {0:m' minutes, 's' seconds'}", TimeSpan.FromMilliseconds(request.Timeout)), webex);
          default:
            var resp = CreateResponseMessage((HttpWebResponse)webex.Response, request);
            throw new HttpException(resp);
        }
      }
    }

    private HttpResponseMsg CreateResponseMessage(HttpWebResponse webResponse, HttpRequestMessage request)
    {
      var httpResponseMessage = new HttpResponseMsg(webResponse.StatusCode);
      httpResponseMessage.ReasonPhrase = webResponse.StatusDescription;
      httpResponseMessage.Version = webResponse.ProtocolVersion;
      httpResponseMessage.RequestMessage = request;

      var content = new System.IO.MemoryStream();
      using (var stream = webResponse.GetResponseStream())
      {
        stream.CopyTo(content);
      }
      content.Position = 0;

      httpResponseMessage.Content = new ResponseContent(content);
      request.RequestUri = webResponse.ResponseUri;
      WebHeaderCollection headers = webResponse.Headers;
      HttpContentHeaders headers2 = httpResponseMessage.Content.Headers;
      HttpResponseHeaders headers3 = httpResponseMessage.Headers;
      if (webResponse.ContentLength >= 0L)
      {
        headers2.ContentLength = new long?(webResponse.ContentLength);
      }

      for (int i = 0; i < headers.Count; i++)
      {
        string key = headers.GetKey(i);
        if (string.Compare(key, "Content-Length", StringComparison.OrdinalIgnoreCase) != 0)
        {
          string[] values = headers.GetValues(i);
          if (!headers3.TryAddWithoutValidation(key, values))
          {
            bool flag = headers2.TryAddWithoutValidation(key, values);
          }
        }
      }
      webResponse.Close();
      return httpResponseMessage;
    }

    private HttpWebRequest CreateAndPrepareWebRequest(HttpRequestMessage request)
    {
      var httpWebRequest = (HttpWebRequest)System.Net.WebRequest.Create(request.RequestUri);
      httpWebRequest.Method = request.Method.Method;
      httpWebRequest.ProtocolVersion = request.Version;
      this.SetDefaultOptions(httpWebRequest);
      SyncClientHandler.SetConnectionOptions(httpWebRequest, request);
      this.SetServicePointOptions(httpWebRequest, request);
      SyncClientHandler.SetRequestHeaders(httpWebRequest, request);
      SyncClientHandler.SetContentHeaders(httpWebRequest, request);
      return httpWebRequest;
    }

    private void SetDefaultOptions(HttpWebRequest webRequest)
    {
      webRequest.Timeout = -1;
      webRequest.AllowAutoRedirect = this.AllowAutoRedirect;
      webRequest.AutomaticDecompression = this.AutomaticDecompression;
      webRequest.PreAuthenticate = this.PreAuthenticate;
      if (this.UseDefaultCredentials)
      {
        webRequest.UseDefaultCredentials = true;
      }
      else
      {
        webRequest.Credentials = this.Credentials;
      }

      if (this.AllowAutoRedirect)
      {
        webRequest.MaximumAutomaticRedirections = this.MaxAutomaticRedirections;
      }

      if (this.UseProxy)
      {
        if (this.Proxy != null)
        {
          webRequest.Proxy = this.Proxy;
        }
      }
      else
      {
        webRequest.Proxy = null;
      }

      if (this.UseCookies)
      {
        webRequest.CookieContainer = this.CookieContainer;
      }
    }

    private static void SetConnectionOptions(HttpWebRequest webRequest, HttpRequestMessage request)
    {
      if (request.Version <= HttpVersion.Version10)
      {
        var keepAlive = false;
        foreach (var current in request.Headers.Connection)
        {
          if (string.Compare(current, "Keep-Alive", StringComparison.OrdinalIgnoreCase) == 0)
          {
            keepAlive = true;
            break;
          }
        }

        webRequest.KeepAlive = keepAlive;
        return;
      }

      if (request.Headers.ConnectionClose == true)
      {
        webRequest.KeepAlive = false;
      }
    }

    private void SetServicePointOptions(HttpWebRequest webRequest, HttpRequestMessage request)
    {
      var headers = request.Headers;
      var expectContinue = headers.ExpectContinue;
      if (expectContinue.HasValue)
      {
        var servicePoint = webRequest.ServicePoint;
        if (servicePoint != null)
          servicePoint.Expect100Continue = expectContinue.Value;
      }
    }

    private static void SetRequestHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
    {
      var dest = webRequest.Headers;
      var origin = request.Headers;
      var hasExpect = origin.Contains("Expect");
      var hasTransferEncoding = origin.Contains("Transfer-Encoding");
      var hasConnection = origin.Contains("Connection");

      #if !NET35
      var hasHost = origin.Contains("Host");
      if (hasHost)
      {
        string host = origin.Host;
        if (host != null)
          webRequest.Host = host;
      }
      #endif

      if (hasExpect)
      {
        var headerValue = origin.Expect.ToString();
        if (!string.IsNullOrEmpty(headerValue))
          webRequest.Expect = headerValue;
      }

      if (hasTransferEncoding)
      {
        var headerValue = origin.TransferEncoding.ToString();
        if (!string.IsNullOrEmpty(headerValue))
          webRequest.TransferEncoding = headerValue;
      }

      if (hasConnection)
      {
        var headerValue = origin.Connection.ToString();
        if (!string.IsNullOrEmpty(headerValue))
          webRequest.Connection = headerValue;
      }

      SetHeaders(webRequest, origin);
    }

    private static void SetContentHeaders(HttpWebRequest webRequest, HttpRequestMessage request)
    {
      if (request.Content != null)
      {
        // Store the length somewhere so that the header gets generated.
        var length = request.Content.Headers.ContentLength;
        SetHeaders(webRequest, request.Content.Headers);
      }
    }

    private static void SetHeaders(HttpWebRequest webRequest, IEnumerable<KeyValuePair<string, IEnumerable<string>>> origin)
    {
      foreach (var kvp in origin)
      {
        switch (kvp.Key.ToLowerInvariant())
        {
          case "accept":
            webRequest.Accept = kvp.Value.First();
            break;
          case "connection":
          case "expect":
          case "host":
          case "transfer-encoding":
            // Do nothing
            break;
          case "content-length":
            webRequest.ContentLength = long.Parse(kvp.Value.First());
            break;
          case "content-type":
            webRequest.ContentType = kvp.Value.First();
            break;
          case "if-modified-since":
            webRequest.IfModifiedSince = DateTime.Parse(kvp.Value.First());
            break;
          case "media-type":
            webRequest.MediaType = kvp.Value.First();
            break;
          case "referer":
            webRequest.Referer = kvp.Value.First();
            break;
          case "user-agent":
            webRequest.UserAgent = kvp.Value.First();
            break;
          default:
            foreach (var value in kvp.Value)
            {
              webRequest.Headers.Add(kvp.Key, value);
            }
            break;
        }
      }
    }

#endif
  }
}
