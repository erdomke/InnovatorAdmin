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
    /// Read/Write timeout in milliseconds
    /// </summary>
    int ReadWriteTimeout { get; set; }
    /// <summary>
    /// HTTP request timeout in milliseconds
    /// </summary>
    int Timeout { get; set; }
    /// <summary>
    /// User-Agent string to send with the request
    /// </summary>
    string UserAgent { get; set; }

    /// <summary>
    /// Configure the request so that it can be used for file uploads
    /// </summary>
    void ConfigureForFileUpload();
    /// <summary>
    /// Set the data in the request stream
    /// </summary>
    void SetContent(Action<IStreamWriter> writer, string contentType = null);
    /// <summary>
    /// Set a request header value
    /// </summary>
    void SetHeader(string name, string value);
  }
}
