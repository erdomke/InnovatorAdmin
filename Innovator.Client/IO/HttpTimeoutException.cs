using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Innovator.Client
{
  [Serializable]
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
    public HttpTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    internal HttpTimeoutException(IHttpResponse resp) : base(resp.StatusCode.ToString())
    {
      _resp = resp;
    }

  }
}
