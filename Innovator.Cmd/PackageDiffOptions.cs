using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  [Verb("diff", HelpText = "Generate a patch package based on a diff")]
  class PackageDiffOptions
  {
    [Option("base", HelpText = "Path to package file containing the base items", Required = true)]
    public string BasePackage { get; set; }

    [Option("ours", HelpText = "Path to package file containing our items", Required = true)]
    public string OurPackage { get; set; }

    [Option('o', "output", HelpText = "Path to the file to output", Required = true)]
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
        yield return new Example("Example", new UnParserSettings() { PreferShortName = true }, new PackageDiffOptions()
        {
          BasePackage = "git:///c:/Repo/Path?commit=base&path=AML",
          OurPackage = "git:///c:/Repo/Path?commit=ours&path=AML",
          MultipleDirectories = true,
          CleanOutput = true,
          Output = @"C:\Output\Path\TestPackage.innpkg",
        });
      }
    }
    public int Execute()
    {
      var st = Stopwatch.StartNew();
      try
      {
        Console.WriteLine(Parser.Default.FormatCommandLine(this));
        Console.WriteLine();
        Console.WriteLine(@"{0:hh\:mm\:ss} Getting package information...", st.Elapsed);
        var dirs = SharedOptions.GetDirectories(BasePackage, OurPackage).ToList();

        var script = default(InstallScript);
        Console.Write(@"{0:hh\:mm\:ss} Calculating diffs... ", st.Elapsed);
        using (var prog = new ProgressBar())
        {
          var processor = new MergeProcessor()
          {
            SortDependencies = true
          };
          processor.ProgressChanged += (s, ev) => prog.Report(ev.Progress / 100.0);
          script = processor.Merge(dirs[0], dirs[1]);
        }
        Console.WriteLine("Done.");

        SharedOptions.WritePackage(st, script, Output, MultipleDirectories, CleanOutput);
        Console.WriteLine();
        Console.WriteLine(@"{0:hh\:mm\:ss} Diff succeeded.", st.Elapsed);
        return 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine();
        Console.WriteLine();
        Console.Error.WriteLine(@"{0:hh\:mm\:ss} Diff failed.", st.Elapsed);
        Console.Error.WriteLine(ex.ToString());
        return -1;
      }
    }
  }
}
