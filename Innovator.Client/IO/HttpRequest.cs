using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client
{
  internal class HttpRequest : HttpRequestMessage, IHttpRequest
  {
    public const int DefaultTimeout = 100000;

    public bool Async { get; set; }
    public int Timeout { get; set; }
    public string UserAgent
    {
      get { return Headers.GetValues("User-Agent").FirstOrDefault(); }
      set { SetHeader("User-Agent", value); }
    }

    public HttpRequest() : base()
    {
      this.Async = true;
      this.Version = new Version(1, 1);
      this.Timeout = HttpRequest.DefaultTimeout;
    }

    public void SetHeader(string name, string value)
    {
      if (Headers.Contains(name))
        Headers.Remove(name);
      Headers.TryAddWithoutValidation(name, value);
    }

    private void Test()
    {

    }
  }
}
