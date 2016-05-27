using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Credentials leveraged with Windows authentication (a.k.a. NTML or Kerberos)
  /// </summary>
  public class WindowsCredentials : INetCredentials
  {
    private System.Net.ICredentials _credentials;
    private string _database;

    /// <summary>
    /// An <see cref="System.Net.ICredentials"/> instance with the same user name and password
    /// </summary>
    public System.Net.ICredentials Credentials { get { return _credentials; } }
    /// <summary>
    /// The database to connect to
    /// </summary>
    public string Database { get { return _database; } }

    /// <summary>
    /// Instantiate a <c>WindowsCredentials</c> instance with the current Windows user credentials
    /// </summary>
    public WindowsCredentials(string database)
    {
      _credentials = System.Net.CredentialCache.DefaultCredentials;
      _database = database;
    }
    /// <summary>
    /// Instantiate a <c>WindowsCredentials</c> instance with explicitly provided credentials
    /// </summary>
    public WindowsCredentials(string database, System.Net.ICredentials credentials)
    {
      _credentials = credentials;
      _database = database;
    }
  }
}
