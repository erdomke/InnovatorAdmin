using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Innovator.Client.Connection
{
  class ServerMapping
  {
    private IRemoteConnection _conn;
    private IEnumerable<string> _databases;

    public IRemoteConnection Connection
    {
      get
      {
        if (_conn == null)
        {
          _conn = Factory.Invoke(this);
        }
        return _conn;
      }
    }
    public IEnumerable<string> Databases
    {
      get
      {
        if (_databases == null)
        {
          _databases = this.Connection.GetDatabases();
        }
        return _databases;
      }
    }
    public Endpoints Endpoints { get; private set; }
    public Func<ServerMapping, IRemoteConnection> Factory { get; set; }
    public ServerType Type { get; private set; }
    public string Url { get; private set; }

    public static IEnumerable<ServerMapping> FromXml(string xml)
    {
      var elem = XElement.Parse(xml);
      return elem.Elements("Server").Select(server =>
      {
        var mapping = new ServerMapping();
        mapping.Url = server.Attribute("url").Value;
        if (server.Attribute("auth") != null && server.Attribute("auth").Value == "jwtSession")
        {
          mapping.Type = ServerType.Proxy;
        }
        else
        {
          mapping.Type = ServerType.Aras;
        }
        if (server.Elements("DB").Any())
        {
          mapping._databases = server.Elements("DB").Select(e => e.Attribute("id").Value).ToArray();
        }
        if (server.Elements("endpoint").Any())
        {
          mapping.Endpoints = new Endpoints(server);
        }
        return mapping;
      });
    }
  }

  enum ServerType
  {
    Aras,
    Proxy
  }
}
