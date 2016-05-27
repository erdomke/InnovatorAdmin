using System;
using System.Collections.Generic;

namespace Innovator.Client
{
  /// <summary>
  /// An HTTP connection to an innovator instance (or proxy) that is not located in the same
  /// memory space as the current code
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
    /// <param name="credentials">Credentials used for authenticating to the instance</param>
    /// <param name="async">Whether to perform this action asynchronously</param>
    /// <returns>A promise to return the user ID as a string</returns>
    IPromise<string> Login(ICredentials credentials, bool async);
    /// <summary>
    /// Log in to the database
    /// </summary>
    /// <param name="credentials">Credentials used for authenticating to the instance</param>
    void Login(ICredentials credentials);
    /// <summary>
    /// Log out of the database
    /// </summary>
    /// <param name="unlockOnLogout">Whether to unlock locked items while logging out</param>
    void Logout(bool unlockOnLogout);
    /// <summary>
    /// Log out of the database
    /// </summary>
    /// <param name="unlockOnLogout">Whether to unlock locked items while logging out</param>
    /// <param name="async">Whether to perform this action asynchronously</param>
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

    /// <summary>
    /// Gets a new connection logged in with the same credentials
    /// </summary>
    /// <param name="async">Whether to perform this action asynchronously</param>
    /// <returns>A promise to return a new connection</returns>
    IPromise<IRemoteConnection> Clone(bool async);
  }
}
