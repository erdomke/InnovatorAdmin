using CommandLine;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  [Verb("import", HelpText = "Import a solution from Aras Innovator")]
  class ImportOptions : SharedOptions
  {
    public async Task<int> Execute()
    {
      var st = Stopwatch.StartNew();
      try
      {
        Console.WriteLine(Parser.Default.FormatCommandLine(this));
        Console.WriteLine();
        Console.WriteLine(@"{0:hh\:mm\:ss} Connecting to innovator...", st.Elapsed);
        var conn = await this.GetConnection().ConfigureAwait(false);
        var processor = new InstallProcessor(conn);

        Console.WriteLine(@"{0:hh\:mm\:ss} Reading the install package...", st.Elapsed);
        var script = default(InstallScript);
        if (Path.GetExtension(InputFile) == ".innpkg")
        {
          using (var pkg = InnovatorPackage.Load(InputFile))
            script = pkg.Read();
        }
        else
        {
          var pkg = new ManifestFolder(InputFile);
          var doc = pkg.Read(out var title);
          script = await processor.ConvertManifestXml(doc, title).ConfigureAwait(false);
        }

        Console.Write(@"{0:hh\:mm\:ss} Installing package `{1}`...", st.Elapsed, script.Title);
        using (var prog = new ProgressBar())
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
        Console.WriteLine("Done.");
        Console.WriteLine();
        Console.WriteLine(@"{0:hh\:mm\:ss} Import succeeded.", st.Elapsed);
        return 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine();
        Console.WriteLine();
        Console.Error.WriteLine(@"{0:hh\:mm\:ss} Import failed.", st.Elapsed);
        Console.Error.WriteLine(ex.ToString());
        return -1;
      }
    }
  }
}
