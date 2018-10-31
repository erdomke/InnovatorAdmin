using CommandLine;
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
  class ExportOptions : SharedOptions
  {
    [Option("title", HelpText = "Title of the package")]
    public string Title { get; set; }

    [Option("author", HelpText = "Author name to include with the package metadata")]
    public string Author { get; set; }

    [Option("website", HelpText = "Website URL to include with the package metadata")]
    public string Website { get; set; }

    [Option("description", HelpText = "Description to include with the package metadata")]
    public string Description { get; set; }

    [Option('o', "output", HelpText = "Path to the file to output", Required = true)]
    public string Output { get; set; }

    [Option("multiple", HelpText = "Whether the output should prefer multiple directories")]
    public bool MultipleDirectories { get; set; }

    [Option("clean", HelpText = "Clean the output file/directory before writing the new content")]
    public bool CleanOutput { get; set; }

    public async Task<int> Execute()
    {
      var st = Stopwatch.StartNew();
      try
      {

        Console.WriteLine(Parser.Default.FormatCommandLine(this));
        Console.WriteLine(@"{0:hh\:mm\:ss} Connecting to innovator...", st.Elapsed);
        var conn = await this.GetConnection().ConfigureAwait(false);
        var processor = new ExportProcessor(conn);

        var refsToExport = default(List<ItemReference>);
        var checkDependencies = true;

        if (string.IsNullOrEmpty(this.Package))
        {
          var version = await conn.FetchVersion(true).ConfigureAwait(false);
          var types = ExportAllType.Types.Where(t => t.Applies(version)).ToList();

          Console.Write(@"{0:hh\:mm\:ss} Identifying all metadata items... ", st.Elapsed);
          using (var prog = new ProgressBar())
          {
            var toExport = await SharedUtils.TaskPool(30, (l, m) => prog.Report(l / 100.0), types
              .Select(t => (Func<Task<IReadOnlyResult>>)(() => conn.ApplyAsync(t.ToString(), true, false).ToTask()))
              .ToArray());
            refsToExport = toExport.SelectMany(r => r.Items())
              .Select(i => ItemReference.FromFullItem(i, true))
              .ToList();
          }
          Console.WriteLine("Done.");

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

        Console.Write(@"{0:hh\:mm\:ss} Exporting metadata... ", st.Elapsed);
        using (var prog = new ProgressBar())
        {
          processor.ProgressChanged += (s, e) => prog.Report(e.Progress / 100.0);
          await processor.Export(script, refsToExport, checkDependencies);
        }
        Console.WriteLine("Done.");

        MultipleDirectories = MultipleDirectories || string.Equals(Path.GetExtension(Output), ".mf", StringComparison.OrdinalIgnoreCase);

        if (CleanOutput)
        {
          Console.Write(@"{0:hh\:mm\:ss} Cleaning output... ", st.Elapsed);
          if (MultipleDirectories)
          {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Output));
            Parallel.ForEach(dir.EnumerateFileSystemInfos(), fs =>
            {
              if (fs is DirectoryInfo di)
                di.Delete(true);
              else
                fs.Delete();
            });
          }
          else
          {
            File.Delete(Output);
          }
          Console.WriteLine("Done.");
        }

        Console.Write(@"{0:hh\:mm\:ss} Writing package... ", st.Elapsed);
        switch (Path.GetExtension(Output).ToLowerInvariant())
        {
          case ".mf":
            var manifest = new ManifestFolder(Output);
            manifest.Write(script);
            break;
          case ".innpkg":
            if (MultipleDirectories)
            {
              using (var pkgFolder = new InnovatorPackageFolder(Output))
                pkgFolder.Write(script);
            }
            else
            {
              using (var pkgFile = new InnovatorPackageFile(Output))
                pkgFile.Write(script);
            }
            break;
          default:
            throw new NotSupportedException("Output file type is not supported");
        }
        Console.WriteLine("Done.");

        Console.WriteLine();
        Console.WriteLine(@"{0:hh\:mm\:ss} Export succeeded.", st.Elapsed);
        return 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine();
        Console.WriteLine();
        Console.Error.WriteLine(@"{0:hh\:mm\:ss} Export failed.", st.Elapsed);
        Console.Error.WriteLine(ex.ToString());
        return -1;
      }
    }
  }
}
