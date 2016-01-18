using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class AnonymousCredentials : ICredentials
  {
    private string _database;

    public string Database { get { return _database; } }

    public AnonymousCredentials(string database)
    {
      _database = database;
    }
  }
}
