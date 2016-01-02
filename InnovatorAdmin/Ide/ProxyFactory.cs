using InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using System.Data.SqlClient;

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

    public static IPromise<IEditorProxy> FromConn(ConnectionData conn)
    {
      Func<ConnectionData, IPromise<IEditorProxy>> factory;
      if (_factories.TryGetValue(conn.Type, out factory))
      {
        return factory.Invoke(conn);
      }
      else
      {
        switch (conn.Type)
        {
          case ConnectionType.Innovator:
            return conn.ArasLogin(true)
              .Convert(c => (IEditorProxy)new Editor.ArasEditorProxy(c, conn.ConnectionName)
              {
                ConnData = conn
              });
          case ConnectionType.SqlServer:
            return Promises.Resolved<IEditorProxy>(new Editor.SqlEditorProxy(conn));
        }
        return Promises.Rejected<IEditorProxy>(new NotSupportedException("Unsupported connection type"));
      }
    }
  }
}
