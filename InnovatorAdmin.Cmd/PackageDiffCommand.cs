using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace InnovatorAdmin.Cmd
{
  [Verb("diff", HelpText = "Generate a patch package based on a diff")]
  internal class PackageDiffCommand : ICommand
  {
    [Option("base", HelpText = "Path to package file containing the base items", Required = true)]
    public string BasePackage { get; set; }

    [Option("ours", HelpText = "Path to package file containing our items", Required = true)]
    public string OurPackage { get; set; }

    [Option('o', "outputfile", HelpText = "Path to the file to output", Required = true)]
    public string Output { get; set; }

    [Option("multiple", HelpText = "Whether the output should prefer multiple directories")]
    public bool MultipleDirectories { get; set; }

    [Option("clean", HelpText = "Clean the output file/directory before writing the new content")]
    public bool CleanOutput { get; set; }

    [Option("sort-suggest", HelpText = "An XML file which controls how the results are sorted.")]
    public string SortSuggest { get; set; }

    [Option("sort-first-of-type", HelpText = "Comma delimited list of IDs that should be sorted at the beginning of their type")]
    public string FirstOfGroup { get; set; }

    [Usage(ApplicationAlias = "InnovatorAdmin.Cmd")]
    public static IEnumerable<Example> Examples
    {
      get
      {
        yield return new Example("Example", new UnParserSettings() { PreferShortName = true }, new PackageDiffCommand()
        {
          BasePackage = "git:///c:/Repo/Path?commit=base&path=AML",
          OurPackage = "git:///c:/Repo/Path?commit=ours&path=AML",
          MultipleDirectories = true,
          CleanOutput = true,
          Output = @"C:\Output\Path\TestPackage.innpkg",
        });
      }
    }

    public Task<int> Execute(ILogger logger)
    {
      var dirs = Package.Create(BasePackage, OurPackage).ToList();

      var script = default(InstallScript);

      var progress = new ProgressLogger(logger);
      var processor = new MergeProcessor();
      if (!string.IsNullOrEmpty(FirstOfGroup))
        processor.Sorter.FirstOfGroup.UnionWith(FirstOfGroup.Split(','));
      if (!string.IsNullOrEmpty(SortSuggest))
        processor.Sorter.Load(XElement.Load(SortSuggest));
      processor.ProgressChanged += progress.Report;
      script = processor.Merge(dirs[0], dirs[1]);
      
      SharedOptions.WritePackage(script, Output, MultipleDirectories, CleanOutput);
      return Task.FromResult(0);
    }
  }
}
