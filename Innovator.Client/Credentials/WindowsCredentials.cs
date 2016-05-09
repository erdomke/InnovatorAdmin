using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Innovator.Client
{
  public class WindowsCredentials : INetCredentials
  {
    private System.Net.ICredentials _credentials;
    private string _database;

    public System.Net.ICredentials Credentials { get { return _credentials; } }
    public string Database { get { return _database; } }

    public WindowsCredentials(string database)
    {
      _credentials = System.Net.CredentialCache.DefaultCredentials;
      _database = database;
    }
    public WindowsCredentials(string database, System.Net.ICredentials credentials)
    {
      _credentials = credentials;
      _database = database;
    }
  }
}
