using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
#if SERIALIZATION
using System.Runtime.Serialization;
#endif
using System.Text;

namespace Innovator.Client
{
#if SERIALIZATION
  [Serializable]
#endif
  public class HttpException : Exception
  {
    private IHttpResponse _resp;

    public IHttpResponse Response
    {
      get { return _resp; }
    }

    public HttpException() : base() { }
    public HttpException(string message) : base(message) { }
    public HttpException(string message, Exception innerException) : base(message, innerException) { }
#if SERIALIZATION
    public HttpException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    internal HttpException(IHttpResponse resp) : base(resp.StatusCode.ToString())
    {
      _resp = resp;
    }

  }
}
