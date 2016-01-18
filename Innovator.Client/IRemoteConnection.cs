using System;
using System.Collections.Generic;

namespace Innovator.Client
{
  /// <summary>
  /// An HTTP connection to an innovator instance (or proxy)
  /// </summary>
  public interface IRemoteConnection : IAsyncConnection, IDisposable
  {
    /// <summary>
    /// URL that the instance resides at
    /// </summary>
    Uri Url { get; }
    
    /// <summary>
    /// Log in to the database asynchronosuly
    /// </summary>
    /// <param name="database">Name of the database</param>
    /// <param name="username">User name</param>
    /// <param name="password">Password (can be passed as a <see cref="String"/>)</param>
    /// <param name="async">Whether to perform this action asynchronously</param>
    /// <returns>A promise to return the user ID as a string</returns>
    IPromise<string> Login(ICredentials credentials, bool async);
    /// <summary>
    /// Log in to the database
    /// </summary>
    /// <param name="database">Name of the database</param>
    /// <param name="username">User name</param>
    /// <param name="password">Password (can be passed as a <see cref="String"/>)</param>
    void Login(ICredentials credentials);
    void Logout(bool unlockOnLogout);
    void Logout(bool unlockOnLogout, bool async);
    /// <summary>
    /// Use a method to configure each outgoing HTTP request
    /// </summary>
    /// <param name="settings">Action used to configure the request</param>
    void DefaultSettings(Action<IHttpRequest> settings);

    /// <summary>
    /// Returns a set of databases which can be connected to using this URL
    /// </summary>
    /// <returns>A set of databases which can be connected to using this URL</returns>
    IEnumerable<string> GetDatabases();
  }
}
