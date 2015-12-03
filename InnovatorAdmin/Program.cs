using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  static class Program
  {
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
      if (string.IsNullOrEmpty(amlStudioStart))
      {
        Application.Run(new AppContext(new Main()));
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
