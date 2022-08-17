using Innovator.Client;
using InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace InnovatorAdmin
{
  public static class ProxyFactory
  {
    private static Dictionary<ConnectionType, Func<ConnectionData, IPromise<IEditorProxy>>> _factories
      = new Dictionary<ConnectionType, Func<ConnectionData, IPromise<IEditorProxy>>>();

    public static void SetDefault(ConnectionType type, Func<ConnectionData, IPromise<IEditorProxy>> factory)
    {
      _factories[type] = factory;
    }

    public static async Task<IEditorProxy> FromConn(ConnectionData conn)
    {
      if (_factories.TryGetValue(conn.Type, out var factory))
        return await factory.Invoke(conn);
      else
        return new Editor.ArasEditorProxy(await conn.ArasLogin(true), conn);        
    }


    private static void Validation(object sender, ValidationEventArgs e)
    {
      // Do nothing
    }
  }
}
