using InnovatorAdmin.Connections;
using System;
using System.IO;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  internal class ConnectionManager
  {
    private ConnectionLibrary _connections;

    private ConnectionManager() { }

    public ConnectionLibrary Library
    {
      get
      {
        if (_connections == null)
        {
          var path = GetConnectionFilePath();
          if (File.Exists(path))
          {
            _connections = ConnectionLibrary.FromFile(path);
          }
          else
          {
            _connections = new ConnectionLibrary();
          }
          if (_connections.Connections.Count == 0)
          {
            _connections.Connections.Add(new ConnectionData()
            {
              ConnectionName = "New Connection"
            });
          }
        }
        return _connections;
      }
    }

    public void Save()
    {
      _connections.Save(GetConnectionFilePath());
    }

    private static ConnectionManager _current = new ConnectionManager();
    public static ConnectionManager Current
    {
      get { return _current; }
    }

    internal static string GetConnectionFilePath()
    {
      return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Innovator Admin", "connections.xml");
    }
  }
}
