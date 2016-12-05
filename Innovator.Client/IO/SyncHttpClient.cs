using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client
{
  internal class SyncHttpClient : HttpClient
  {
    private SyncClientHandler _handler;

    public SyncHttpClient() : this(new SyncClientHandler()) { }
    public SyncHttpClient(SyncClientHandler handler) : base(handler)
    {
      _handler = handler;
    }

#if HTTPSYNC
    public HttpResponseMsg Send(HttpRequest request)
    {
      return _handler.Send(request);
    }
#endif
  }
}
