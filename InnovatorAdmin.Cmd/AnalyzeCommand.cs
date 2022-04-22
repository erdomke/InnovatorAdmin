using CommandLine;
using System;
using System.Linq;

namespace InnovatorAdmin.Cmd
{
  [Verb("analyze", HelpText = "Analyze a package file for potential issues")]
  internal class AnalyzeCommand
  {
    [Option('f', "inputfile", HelpText = "Path to package file containing the items to import/export")]
    public string InputFile { get; set; }

    [Option("removemeta", HelpText = "Remove package metadata")]
    public bool RemoveMetadata { get; set; }

    public int Execute()
    {
      return ConsoleTask.Execute(this, console =>
      {
        console.WriteLine("Getting package information...");
        var package = Package.Create(InputFile).Single();
        var exceptions = package.TryRead(out var script).ToList();

        var sorter = new DependencySorter();
        var lines = sorter.SortByDependencies(script.Lines, PackageMetadataProvider.FromScript(script), 1);
        foreach (var line in lines
          .Where(l => l.Type == InstallType.DependencyCheck)
          .OrderBy(l => l.Reference.Origin?.Type)
          .ThenBy(l => l.Reference.Origin?.KeyedName))
          exceptions.Add(new InvalidOperationException($"Unresolved dependency found in {line.Reference.Origin?.Type} {line.Reference.Origin?.KeyedName} on the item {line.Reference.Type} {line.Reference.Unique}"));

        foreach (var exception in exceptions)
        {
          console.WriteLine(exception.Message);
          foreach (var dataKey in exception.Data.Keys)
            console.WriteLine($"  {dataKey} = {exception.Data[dataKey]}");
        }
      });
    }
  }
}
