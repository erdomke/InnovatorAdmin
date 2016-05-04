using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client.Connection
{
  public interface IArasConnection : IAsyncConnection
  {
    List<Action<IHttpRequest>> DefaultSettings { get; }
    CompressionType Compression { get; }
    int Version { get; }
    void SetDefaultHeaders(Action<string, string> writer);
  }
}
