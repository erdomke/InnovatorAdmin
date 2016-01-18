using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace Innovator.Client.Connection
{
  public class DefaultHttpService : IHttpService
  {
    private static TraceSource _httpTracing;

    public static TraceSource PerformanceTrace
    {
      get { return _httpTracing; }
    }

    static DefaultHttpService()
    {
      _httpTracing = new TraceSource("Innovator.Client.HttpCalls");
      _httpTracing.Switch = new SourceSwitch("Innovator.Client.HttpCallsLevel");
    }

    public const int DefaultTimeout = 100000;

    private CookieContainer _cookieContainer = new CookieContainer();
    
    public bool AllowAutoRedirect { get; set; }
    public bool AutomaticDecompression { get; set; }
    public CompressionType Compression { get; set; }
    public bool StoreCookies { get; set; }

    public DefaultHttpService()
    {
      this.AllowAutoRedirect = true;
      this.AutomaticDecompression = true;
      this.Compression = CompressionType.gzip;
      this.StoreCookies = true;
    }

    public virtual IPromise<IHttpResponse> Execute(string method, string baseUrl, QueryString queryString, System.Net.ICredentials credentials, bool async, Action<IHttpRequest> configure)
    {
      var uri = baseUrl + (queryString == null ? "" : queryString.ToString());

      var req = (HttpWebRequest)System.Net.WebRequest.Create(uri);
      req.AllowAutoRedirect = this.AllowAutoRedirect;
      if (this.AutomaticDecompression) req.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);
      if (this.StoreCookies) req.CookieContainer = _cookieContainer;
      req.Method = method;
      req.ReadWriteTimeout = 300000;
      req.Timeout = DefaultTimeout;
      if (method == "POST")
      {
        req.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
      }
      req.Credentials = credentials;
      var result = new WebRequest(req, this.Compression);
      if (configure != null) configure.Invoke(result);

      var st = Stopwatch.StartNew();
      return result.Execute(async).Convert(r => {
        if (r.Core.Cookies.Count > 0) SetCookies(r.Core.Cookies);
        return (IHttpResponse)r;
      }).Always(() => {
        st.Stop();
        _httpTracing.TraceData(TraceEventType.Verbose, 0, st.ElapsedMilliseconds);
      });
    }

    protected virtual void SetCookies(CookieCollection cookie)
    {
      // Do nothing by default
    }
  }
}
