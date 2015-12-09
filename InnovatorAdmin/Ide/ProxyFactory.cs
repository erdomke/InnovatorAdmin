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
    public static IPromise<IEditorProxy> FromConn(ConnectionData conn)
    {
      switch (conn.Type)
      {
        case ConnectionType.Innovator:
          return conn.ArasLogin(true)
            .Convert(c => (IEditorProxy)new ArasEditorProxy(c, conn.ConnectionName)
            {
              ConnData = conn
            });
        case ConnectionType.SqlServer:
          return Promises.Resolved<IEditorProxy>(new SqlEditorProxy(conn));
      }
      return Promises.Rejected<IEditorProxy>(new NotSupportedException());
    }
  }
}
