using CommandLine;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  [Verb("import", HelpText = "Import a solution from Aras Innovator")]
  internal class ImportCommand : SharedOptions, ICommand
  {
    public Task<int> Execute(ILogger logger)
    {
      return ConsoleTask.ExecuteAsync(this, async (console) =>
      {
        console.WriteLine("Connecting to innovator...");
        var conn = await this.GetConnection().ConfigureAwait(false);
        var processor = new InstallProcessor(conn, logger);

        console.WriteLine("Reading the install package...");
        var script = default(InstallScript);
        if (Path.GetExtension(InputFile) == ".innpkg")
        {
          using (var pkg = Package.Create(InputFile).Single())
            script = pkg.Read();
        }
        else
        {
          var pkg = new ManifestFolder(InputFile);
          var doc = pkg.Read(out var title);
          script = await processor.ConvertManifestXml(doc, title).ConfigureAwait(false);
        }

        console.Write("Installing package `{0}`...", script.Title);
        using (var prog = console.Progress())
        {
          var tcs = new TaskCompletionSource<int>();
          processor.ProgressChanged += (s, e) => prog.Report(e.Progress / 100.0);
          processor.ErrorRaised += (s, e) =>
          {
            tcs.TrySetException(e.Exception);
            e.RecoveryOption = RecoveryOption.Abort;
          };
          processor.ActionComplete += (s, e) =>
          {
            if (e.Exception == null)
              tcs.TrySetResult(0);
            else
              tcs.TrySetException(e.Exception);
          };
          await processor.Initialize(script).ConfigureAwait(false);
          processor.Install();
          await tcs.Task.ConfigureAwait(false);
        }
        console.WriteLine("Done.");
      });
    }
  }
}
