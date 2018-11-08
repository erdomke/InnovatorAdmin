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
  internal class ConvertCommand
  {
    [Option('f', "inputfile", HelpText = "Path to package file containing the items to import/export")]
    public string InputFile { get; set; }

    [Option('o', "outputfile", HelpText = "Path to the file to output", Required = true)]
    public string Output { get; set; }

    [Option("multiple", HelpText = "Whether the output should prefer multiple directories")]
    public bool MultipleDirectories { get; set; }

    [Option("clean", HelpText = "Clean the output file/directory before writing the new content")]
    public bool CleanOutput { get; set; }

    public int Execute()
    {
      return ConsoleTask.Execute(this, (console) =>
      {
        console.WriteLine("Getting package information...");
        var script = InnovatorPackage.Load(InputFile).Read();
        SharedOptions.WritePackage(console, script, Output, MultipleDirectories, CleanOutput);
      });
    }
  }
}
