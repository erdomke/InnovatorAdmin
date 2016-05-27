using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  /// <summary>
  /// Credentials for a user that is not authenticated
  /// </summary>
  public class AnonymousCredentials : ICredentials
  {
    private string _database;

    /// <summary>
    /// The database to connect to
    /// </summary>
    public string Database { get { return _database; } }

    /// <summary>
    /// Instantiate an <c>AnonymousCredentials</c> instance
    /// </summary>
    public AnonymousCredentials(string database)
    {
      _database = database;
    }
  }
}
