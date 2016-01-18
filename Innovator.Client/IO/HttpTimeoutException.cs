using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Innovator.Client
{
  public class HttpTimeoutException : Exception
  {
    public HttpTimeoutException() : base("HTTP request timed out") { }
    public HttpTimeoutException(string message) : base(message) { }
    public HttpTimeoutException(string message, Exception innerException) : base(message, innerException) { }
    public HttpTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
