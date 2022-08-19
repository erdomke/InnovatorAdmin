using CommandLine;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  [Verb("convert", HelpText = "Convert a package from one format to another")]
  internal class ConvertCommand : ICommand
  {
    [Option('f', "inputfile", HelpText = "Path to package file containing the items to import/export")]
    public string InputFile { get; set; }

    [Option('o', "outputfile", HelpText = "Path to the file to output", Required = true)]
    public string Output { get; set; }

    [Option("multiple", HelpText = "Whether the output should prefer multiple directories")]
    public bool MultipleDirectories { get; set; }

    [Option("clean", HelpText = "Clean the output file/directory before writing the new content")]
    public bool CleanOutput { get; set; }

    public Task<int> Execute(ILogger logger)
    {
      var script = Package.Create(InputFile).Single().Read();
      SharedOptions.WritePackage(script, Output, MultipleDirectories, CleanOutput);
      return Task.FromResult(0);
    }
  }
}
