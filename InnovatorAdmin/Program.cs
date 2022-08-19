using Innovator.Client;
using Innovator.Telemetry;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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
    private static string _logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Innovator Admin", "logs");

    public static int PortNumber { get { return _serverPort; } }

    private static int FreeTcpPort()
    {
      var l = new System.Net.Sockets.TcpListener(System.Net.IPAddress.Loopback, 0);
      l.Start();
      var port = ((System.Net.IPEndPoint)l.LocalEndpoint).Port;
      l.Stop();
      return port;
    }

    private static void CleanupOldLogs()
    {
      foreach (var toDelete in Directory.GetFiles(_logDirectory, "*.log")
        .OrderByDescending(p => p)
        .Skip(5))
      {
        try
        {
          File.Delete(toDelete);
        }
        catch (Exception ex)
        {
          Utils.Logger.LogWarning(ex, "Error cleaning up old log files");
        }
      }
    }

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      var resourceBuilder = ResourceBuilder.CreateDefault().AddService("InnovatorAdmin");
      var tags = new Dictionary<string, object>
      {
        ["arguments"] = args,
        ["working_directory"] = Directory.GetCurrentDirectory(),
        ["os_version"] = Environment.OSVersion.ToString(),
        ["machine_name"] = Environment.MachineName,
        ["user"] = $"{Environment.UserDomainName}\\{Environment.UserName}"
      };

      var logPath = Path.Combine(_logDirectory, $"InnovatorAdmin-{DateTime.UtcNow:yyyyMMddHHmmss}.log");
      Directory.CreateDirectory(Path.GetDirectoryName(logPath));
      using var logWriter = new StreamWriter(logPath);

      var enricher = new DefaultEnricher()
        .WithExceptionEnricher<ServerException>(SharedUtils.EnrichServerException);
      var sslogWriter = new SslogWriter(logWriter);
      using (var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .AddSource("InnovatorAdmin")
        .SetResourceBuilder(resourceBuilder)
        .AddProcessor(new ActivityProcessor(sslogWriter))
        .Build())
      using (var loggerFactory = LoggerFactory.Create(builder =>
      {
        builder.AddOpenTelemetry(options =>
        {
          options.IncludeFormattedMessage = true;
          options.IncludeScopes = true;
          options.SetResourceBuilder(resourceBuilder);
          options.AddProcessor(new LogProcessor(sslogWriter, enricher));
        });
      }))
      using (SharedUtils.StartActivity("InnovatorAdmin.exe", tags: tags))
      {
        Utils.Logger = loggerFactory.CreateLogger("InnovatorAdmin");
        CleanupOldLogs();
        try
        {
          Application.EnableVisualStyles();
          Application.SetCompatibleTextRenderingDefault(false);

          var amlStudioStart = args.FirstOrDefault(a =>
            a.StartsWith("/amlstudio:", StringComparison.OrdinalIgnoreCase));

          _serverPort = FreeTcpPort();
          using (var server = new WebServer(new[] { "http://localhost:" + _serverPort.ToString() + "/" }))
          {
            server.Run();
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
        catch (Exception ex)
        {
          Utils.Logger.LogError(ex, null);
          MessageBox.Show(ex.ToString(), "Innovator Admin: Crash");
        }
      }
    }

    public static string AssemblyPath
    {
      get
      {
        return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);
      }
    }
  }
}
