using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Innovator.Client
{
  public interface IHttpResponse
  {
    IDictionary<string, string> Headers { get; }
    HttpStatusCode StatusCode { get; }
    Stream AsStream { get; }
  }
}
