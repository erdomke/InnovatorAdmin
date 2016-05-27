using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Innovator.Client
{
  /// <summary>
  /// Credentials with a specified user name and password provided by the user
  /// </summary>
  public class ExplicitCredentials : INetCredentials
  {
    private string _database;
    private SecureToken _password;
    private string _username;

#if NET4
    /// <summary>
    /// An <see cref="System.Net.ICredentials"/> instance with the same user name and password
    /// </summary>
    public System.Net.ICredentials Credentials { get { return new NetworkCredential(_username, _password); } }
#else
    public System.Net.ICredentials Credentials { get { return new NetworkCredential(_username
      , _password.UseString((ref string s) => new string(s.ToCharArray()))); } }
#endif
    /// <summary>
    /// The database to connect to
    /// </summary>
    public string Database { get { return _database; } }
    /// <summary>
    /// The password to use
    /// </summary>
    public SecureToken Password { get { return _password; } }
    /// <summary>
    /// The user name to use
    /// </summary>
    public string Username { get { return _username; } }

    /// <summary>
    /// Instantiate an <c>ExplicitCredentials</c> instance
    /// </summary>
    public ExplicitCredentials(string database, string username, SecureToken password)
    {
      _database = database;
      _username = username;
      _password = password;
    }
  }
}
