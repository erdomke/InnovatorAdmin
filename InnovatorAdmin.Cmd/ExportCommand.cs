using CommandLine;
using CommandLine.Text;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  [Verb("export", HelpText = "Export a solution from Aras Innovator")]
  internal class ExportCommand : SharedOptions
  {
    [Option("title", HelpText = "Title of the package")]
    public string Title { get; set; }

    [Option("author", HelpText = "Author name to include with the package metadata")]
    public string Author { get; set; }

    [Option("website", HelpText = "Website URL to include with the package metadata")]
    public string Website { get; set; }

    [Option("description", HelpText = "Description to include with the package metadata")]
    public string Description { get; set; }

    [Option('o', "outputfile", HelpText = "Path to the file to output", Required = true)]
    public string Output { get; set; }

    [Option("multiple", HelpText = "Whether the output should prefer multiple directories")]
    public bool MultipleDirectories { get; set; }

    [Option("clean", HelpText = "Clean the output file/directory before writing the new content")]
    public bool CleanOutput { get; set; }

    [Usage(ApplicationAlias = "InnovatorAdmin.Cmd")]
    public static IEnumerable<Example> Examples
    {
      get
      {
        yield return new Example("Example", new UnParserSettings() { PreferShortName = true }, new ExportCommand()
        {
          Url = "http://localhost/InnovatorServer/",
          Database = "InnovatorSolutions",
          Username = "admin",
          Password = "innovator",
          Output = @"C:\Output\Path\TestPackage.innpkg",
          MultipleDirectories = true,
          CleanOutput = true
        });
      }
    }

    public Task<int> Execute()
    {
      return ConsoleTask.ExecuteAsync(this, async (console) =>
      {
        console.WriteLine("Connecting to innovator...");
        var conn = await this.GetConnection().ConfigureAwait(false);
        var processor = new ExportProcessor(conn);

        var refsToExport = default(List<ItemReference>);
        var checkDependencies = true;

        if (string.IsNullOrEmpty(this.InputFile))
        {
          var version = await conn.FetchVersion(true).ConfigureAwait(false);
          var types = ExportAllType.Types.Where(t => t.Applies(version)).ToList();

          console.Write("Identifying all metadata items... ");
          using (var prog = console.Progress())
          {
            var toExport = await SharedUtils.TaskPool(30, (l, m) => prog.Report(l / 100.0), types
              .Select(t => (Func<Task<IReadOnlyResult>>)(() => conn.ApplyAsync(t.ToString(), true, false).ToTask()))
              .ToArray());
            refsToExport = toExport.SelectMany(r => r.Items())
              .Select(i => ItemReference.FromFullItem(i, true))
              .ToList();
          }
          console.WriteLine("Done.");

          checkDependencies = false;
        }
        else
        {
          throw new NotSupportedException("Input package is not supported");
        }

        var script = new InstallScript
        {
          ExportUri = new Uri(Url),
          ExportDb = Database,
          Lines = Enumerable.Empty<InstallItem>(),
          Title = Title ?? System.IO.Path.GetFileNameWithoutExtension(Output),
          Creator = Author ?? Username,
          Website = string.IsNullOrEmpty(Website) ? null : new Uri(Website),
          Description = Description,
          Created = DateTime.Now,
          Modified = DateTime.Now
        };

        console.Write("Exporting metadata... ");
        using (var prog = console.Progress())
        {
          processor.ProgressChanged += (s, e) => prog.Report(e.Progress / 100.0);
          processor.ActionComplete += (s, e) =>
          {
            if (e.Exception != null)
              throw new AggregateException(e.Exception);
          };
          await processor.Export(script, refsToExport, checkDependencies);
        }
        console.WriteLine("Done.");

        WritePackage(console, script, Output, MultipleDirectories, CleanOutput);
      });
    }
  }
}
