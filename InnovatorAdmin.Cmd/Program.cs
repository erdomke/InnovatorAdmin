using CommandLine;
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
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  class Program
  {
    static async Task<int> Main(string[] args)
    {
      var resourceBuilder = ResourceBuilder.CreateDefault().AddService("InnovatorAdmin.Cmd");
      var tags = new Dictionary<string, object>
      {
        ["arguments"] = args,
        ["working_directory"] = Directory.GetCurrentDirectory(),
        ["os_version"] = Environment.OSVersion.ToString(),
        ["machine_name"] = Environment.MachineName,
        ["user"] = $"{Environment.UserDomainName}\\{Environment.UserName}"
      };

      var enricher = new DefaultEnricher()
        .WithExceptionEnricher<ServerException>(SharedUtils.EnrichServerException);
      var consoleWriter = new SslogWriter(Console.Out)
      {
        UseConsoleColors = true
      };
      using (var tracerProvider = Sdk.CreateTracerProviderBuilder()
        .AddSource("InnovatorAdmin")
        .SetResourceBuilder(resourceBuilder)
        .AddProcessor(new ActivityProcessor(consoleWriter))
        .Build())
      using (var loggerFactory = LoggerFactory.Create(builder =>
      {
        builder.AddOpenTelemetry(options =>
        {
          options.IncludeFormattedMessage = true;
          options.IncludeScopes = true;
          options.SetResourceBuilder(resourceBuilder);
          options.AddProcessor(new LogProcessor(consoleWriter, enricher));
        });
      }))
      using (SharedUtils.StartActivity("InnovatorAdmin.Cmd.exe", tags: tags))
      {
        var logger = loggerFactory.CreateLogger("InnovatorAdmin.Cmd");


        //var parser = new Parser(with => with.IgnoreUnknownArguments = false);
        var commands = new[]
        {
          typeof(ExportCommand),
          typeof(ImportCommand),
          typeof(PackageDiffCommand),
          typeof(ConvertCommand),
          typeof(ApplyCommand),
          typeof(AnalyzeCommand),
          typeof(DocumentationCommand)
        };
        var task = default(Task<int>);
        var cmdArgs = Parser.Default.ParseArguments(args, commands);
        cmdArgs
          .WithNotParsed(errors => task = TryParseArasFormat((NotParsed<object>)cmdArgs, args, logger))
          .WithParsed(options => task = Execute((ICommand)options, logger));

        try
        {
          return await task;
        }
        catch (Exception ex)
        {
          logger.LogError(ex, null);
          return -1;
        }
      }
    }

    private static async Task<int> Execute(ICommand command, ILogger logger)
    {
      return await command.Execute(logger);
    }

    private static Task<int> TryParseArasFormat(NotParsed<object> result, string[] args, ILogger logger)
    {
      var opts = default(SharedOptions);
      if (result.TypeInfo.Choices.Any())
      {
        opts = new ExportCommand();
      }
      else if (typeof(SharedOptions).IsAssignableFrom(result.TypeInfo.Current))
      {
        opts = (SharedOptions)Activator.CreateInstance(result.TypeInfo.Current);
      }
      else
      {
        return Task.FromResult(-1);
      }

      foreach (var arg in args)
      {
        var idx = arg.IndexOf('=');
        if (idx > 0)
        {
          switch (arg.Substring(0, idx).ToLowerInvariant())
          {
            case "server":
              opts.Url = arg.Substring(idx + 1);
              break;
            case "database":
              opts.Database = arg.Substring(idx + 1);
              break;
            case "login":
              opts.Username = arg.Substring(idx + 1);
              break;
            case "password":
              opts.Password = arg.Substring(idx + 1);
              break;
            case "mffile":
              opts.InputFile = arg.Substring(idx + 1);
              break;
            case "dir":
              if (opts is ExportCommand e2)
                e2.Output = arg.Substring(idx + 1);
              break;
          }
        }
      }

      if (string.IsNullOrEmpty(opts.Url)
        || string.IsNullOrEmpty(opts.Database))
        return Task.FromResult(-1);

      if (opts is ExportCommand export)
        return Execute(export, logger);
      else if (opts is ImportCommand import)
        return Execute(import, logger);
      return Task.FromResult(-1);
    }
  }
}
