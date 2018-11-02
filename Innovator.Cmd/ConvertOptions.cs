using CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  [Verb("convert", HelpText = "Convert a package from one format to another")]
  class ConvertOptions
  {
    [Option('f', "inputfile", HelpText = "Path to package file containing the items to import/export")]
    public string InputFile { get; set; }

    [Option('o', "output", HelpText = "Path to the file to output", Required = true)]
    public string Output { get; set; }

    [Option("multiple", HelpText = "Whether the output should prefer multiple directories")]
    public bool MultipleDirectories { get; set; }

    [Option("clean", HelpText = "Clean the output file/directory before writing the new content")]
    public bool CleanOutput { get; set; }

    public int Execute()
    {
      var st = Stopwatch.StartNew();
      try
      {
        Console.WriteLine(Parser.Default.FormatCommandLine(this));
        Console.WriteLine();
        Console.WriteLine(@"{0:hh\:mm\:ss} Getting package information...", st.Elapsed);
        var script = InnovatorPackage.Load(InputFile).Read();
        SharedOptions.WritePackage(st, script, Output, MultipleDirectories, CleanOutput);

        Console.WriteLine();
        Console.WriteLine(@"{0:hh\:mm\:ss} Convert succeeded.", st.Elapsed);
        return 0;
      }
      catch (Exception ex)
      {
        Console.WriteLine();
        Console.WriteLine();
        Console.Error.WriteLine(@"{0:hh\:mm\:ss} Convert failed.", st.Elapsed);
        Console.Error.WriteLine(ex.ToString());
        return -1;
      }
    }
  }
}
