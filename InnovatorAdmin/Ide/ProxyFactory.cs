using Innovator.Client;
using InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
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
              .Convert(c => (IEditorProxy)new Editor.ArasEditorProxy(c, conn));
        }
        return Promises.Rejected<IEditorProxy>(new NotSupportedException("Unsupported connection type"));
      }
    }


    private static void Validation(object sender, ValidationEventArgs e)
    {
      // Do nothing
    }
  }
}
