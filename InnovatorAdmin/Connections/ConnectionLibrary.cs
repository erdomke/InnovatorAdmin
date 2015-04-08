using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Aras.Tools.InnovatorAdmin.Connections
{
  public class ConnectionLibrary
  {
    public ConnectionLibrary()
    {
      Connections = new List<ConnectionData>();
    }

    public List<ConnectionData> Connections { get; set; }

    public void Save(string path)
    {
      XmlSerializer ser = new XmlSerializer(typeof(ConnectionLibrary));
      Directory.CreateDirectory(Path.GetDirectoryName(path));
      using (var stream = new FileStream(path, FileMode.Create))
      {
        ser.Serialize(stream, this);
      }
    }

    public static ConnectionLibrary FromFile(string path)
    {
      XmlSerializer ser = new XmlSerializer(typeof(ConnectionLibrary));
      using (var stream = new FileStream(path, FileMode.Open))
      {
        return (ConnectionLibrary)ser.Deserialize(stream);
      }
    }
  }
}
