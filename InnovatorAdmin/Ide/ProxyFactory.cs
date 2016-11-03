using InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Innovator.Client;
using System.Data.SqlClient;
using System.Web.Services.Description;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Net.Http;

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
          case ConnectionType.Soap:
            var http = new HttpClient();
            return http.GetStreamAsync(conn.Url).ToPromise()
              .Convert(r =>
              {
                ServiceDescription descrip;
                using (var reader = new StreamReader(r))
                using (var xml = XmlReader.Create(reader))
                {
                  descrip = ServiceDescription.Read(xml);
                }

                return (IEditorProxy)new Editor.SoapEditorProxy(conn, descrip
                  , Editor.XmlSchemas.SchemasFromDescrip(descrip));
              });
          case ConnectionType.SqlServer:
            return Promises.Resolved<IEditorProxy>(new Editor.SqlEditorProxy(conn));
          case ConnectionType.Sharepoint:
            return new Editor.SharepointEditorProxy(conn).Initialize().ToPromise();
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
