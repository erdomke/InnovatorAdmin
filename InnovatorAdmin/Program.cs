using Nancy.Hosting.Self;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  static class Program
  {
    private static int _serverPort;

    public static int PortNumber { get { return _serverPort; } }

    private static int FreeTcpPort()
    {
      var l = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
      l.Start();
      var port = ((System.Net.IPEndPoint)l.LocalEndpoint).Port;
      l.Stop();
      return port;
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      var amlStudioStart = args.FirstOrDefault(a =>
        a.StartsWith("/amlstudio:", StringComparison.OrdinalIgnoreCase));

      try
      {
        _serverPort = FreeTcpPort();
        using (var host = new NancyHost(new Uri("http://localhost:" + _serverPort.ToString())))
        {
          host.Start();
          if (string.IsNullOrEmpty(amlStudioStart))
          {
            Application.Run(new AppContext(new EditorWindow()));
          }
          else
          {
            var connName = amlStudioStart.Substring(11);
            var conn = ConnectionManager.Current.Library.Connections.FirstOrDefault(c =>
              c.ConnectionName.Equals(connName, StringComparison.OrdinalIgnoreCase));
            var win = new EditorWindow();
            if (conn != null) win.SetConnection(conn);
            Application.Run(new AppContext(win));
          }
      }
      }
      catch (Exception) { }   // Eat the error for now
    }

    public static string AssemblyPath
    {
      get
      {
        var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
        var uri = new UriBuilder(codeBase);
        var path = Uri.UnescapeDataString(uri.Path);
        return Path.GetFullPath(path);
      }
    }
  }
}
