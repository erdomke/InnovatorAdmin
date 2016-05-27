using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Interface for a connection to an Aras Innovator instance that allows asynchronous calls
  /// </summary>
  public interface IAsyncConnection : IConnection
  {
    /// <summary>
    /// Calls a SOAP action asynchronously
    /// </summary>
    /// <param name="request">Request AML and possibly files <see cref="UploadCommand"/></param>
    /// <param name="async">Whether to perform this action asynchronously</param>
    /// <returns>A promise to return an XML SOAP response as a string</returns>
    IPromise<System.IO.Stream> Process(Command request, bool async);
  }
}
