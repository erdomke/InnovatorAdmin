using System.Collections.Generic;

namespace Innovator.Client
{
  /// <summary>
  /// Interface for a connection to an Aras Innovator instance
  /// </summary>
  public interface IConnection
  {
    /// <summary>
    /// AML context used for creating AML objects and formatting AML statements
    /// </summary>
    ElementFactory AmlContext { get; }
    /// <summary>
    /// Name of the connected database
    /// </summary>
    string Database { get; }
    /// <summary>
    /// ID of the authenticated user
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// Calls a SOAP action asynchronously
    /// </summary>
    /// <param name="request">Request AML and possibly files <see cref="UploadCommand"/></param>
    /// <returns>An XML SOAP response as a string</returns>
    System.IO.Stream Process(Command request);

    /// <summary>
    /// Creates an upload request used for uploading files to the server
    /// </summary>
    /// <returns>A new upload request used for uploading files to the server</returns>
    UploadCommand CreateUploadCommand();

    /// <summary>
    /// Expands a relative URL to a full URL
    /// </summary>
    /// <param name="relativeUrl">The relative URL</param>
    /// <returns>A full URL relative to the connection</returns>
    string MapClientUrl(string relativeUrl);
  }
}
