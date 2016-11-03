using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// HTTP request being sent to a server (Aras, Proxy, etc.)
  /// </summary>
  public interface IHttpRequest
  {
    /// <summary>
    /// HTTP request timeout in milliseconds
    /// </summary>
    int Timeout { get; set; }

    /// <summary>
    /// User-Agent string to send with the request
    /// </summary>
    string UserAgent { get; set; }

    /// <summary>
    /// Set a request header value
    /// </summary>
    void SetHeader(string name, string value);
  }
}
