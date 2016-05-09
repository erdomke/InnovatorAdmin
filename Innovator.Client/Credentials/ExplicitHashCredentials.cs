using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class ExplicitHashCredentials : ICredentials
  {
    private string _database;
    private string _passwordHash;
    private string _username;

    public string Database { get { return _database; } }
    public string PasswordHash { get { return _passwordHash; } }
    public string Username { get { return _username; } }

    public ExplicitHashCredentials(string database, string username, string passwordHash)
    {
      _database = database;
      _username = username;
      _passwordHash = passwordHash;
    }
  }
}
