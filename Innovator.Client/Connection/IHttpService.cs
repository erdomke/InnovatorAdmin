using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  public interface IHttpService
  {
    bool AllowAutoRedirect { get; set; }
    bool AutomaticDecompression { get; set; }
    CompressionType Compression { get; set; }
    bool StoreCookies { get; set; }

    IPromise<IHttpResponse> Execute(string method, string baseUrl, QueryString queryString, System.Net.ICredentials credentials, bool async, Action<IHttpRequest> configure);
  }
}
