using System;
using System.Collections.Generic;
using System.Linq;
#if SERIALIZATION
using System.Runtime.Serialization;
#endif
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client
{
#if SERIALIZATION
  [Serializable]
#endif
  public class HttpTimeoutException : Exception
  {
    private IHttpResponse _resp;

    public IHttpResponse Response
    {
      get { return _resp; }
    }

    public HttpTimeoutException() : base() { }
    public HttpTimeoutException(string message) : base(message) { }
    public HttpTimeoutException(string message, Exception innerException) : base(message, innerException) { }
#if SERIALIZATION
    public HttpTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    internal HttpTimeoutException(IHttpResponse resp) : base(resp.StatusCode.ToString())
    {
      _resp = resp;
    }

  }
}
