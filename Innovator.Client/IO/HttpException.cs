using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace Innovator.Client
{
  [Serializable]
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
    public HttpException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    internal HttpException(WebException ex, HttpWebRequest req) : base(ex.Message, ex)
    {
      _resp = new WebResponse(req, (HttpWebResponse)ex.Response);
    }
    internal HttpException(IHttpResponse resp) : base(resp.StatusCode.ToString())
    {
      _resp = resp;
    }

  }
}
