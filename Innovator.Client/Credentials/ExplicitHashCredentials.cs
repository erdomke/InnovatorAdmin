using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Credentials with a specified user name and MD5 password hash provided by the user
  /// </summary>
  public class ExplicitHashCredentials : ICredentials
  {
    private string _database;
    private string _passwordHash;
    private string _username;

    /// <summary>
    /// The database to connect to
    /// </summary>
    public string Database { get { return _database; } }
    /// <summary>
    /// The MD5 hash of the password to use
    /// </summary>
    public string PasswordHash { get { return _passwordHash; } }
    /// <summary>
    /// The user name to use
    /// </summary>
    public string Username { get { return _username; } }

    /// <summary>
    /// Instantiate an <c>ExplicitHashCredentials</c> instance
    /// </summary>
    public ExplicitHashCredentials(string database, string username, string passwordHash)
    {
      _database = database;
      _username = username;
      _passwordHash = passwordHash;
    }
  }
}
