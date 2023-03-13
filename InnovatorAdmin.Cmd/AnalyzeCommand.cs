using CommandLine;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  [Verb("analyze", HelpText = "Analyze a package file for potential issues", Hidden = true)]
  internal class AnalyzeCommand : ICommand
  {
    [Option('f', "inputfile", HelpText = "Path to package file containing the items to import/export")]
    public string InputFile { get; set; }

    [Option("log", HelpText = "Path to XML log file")]
    public string LogFile { get; set; }

    [Option("removemeta", HelpText = "Remove package metadata")]
    public bool RemoveMetadata { get; set; }

    public async Task<int> Execute(ILogger logger)
    {
      var package = Package.Create(InputFile).Single();

      if (package.TryRead(logger, out var script))
      {
        var sorter = new DependencySorter();
        var lines = sorter.SortByDependencies(script.Lines, PackageMetadataProvider.FromScript(script), 1);
        var checks = string.Join("\r\n", lines
          .Where(l => l.Type == InstallType.DependencyCheck)
          .Select(l => l.Script.OuterXml));
        File.WriteAllText(@"C:\Users\edomke\AppData\Roaming\Innovator Admin\logs\checks.txt", checks);
        //logger.LogInformation()
        return 0;
      }
      else
      {
        return -1;
      }

      //var sorter = new DependencySorter();
      //var lines = sorter.SortByDependencies(script.Lines, PackageMetadataProvider.FromScript(script), 1);
      //foreach (var group in lines
      //  .Where(l => l.Type == InstallType.DependencyCheck)
      //  .OrderBy(l => l.Reference.Origin?.Type)
      //  .ThenBy(l => l.Reference.Origin?.KeyedName)
      //  .GroupBy(l => l.Reference.Origin))
      //{
      //  logger.Event("PackageError.UnresolvedReference", writer =>
      //  {
      //    var origin = lines.FirstOrDefault(l => l.Reference == group.Key);
      //    writer.WriteAttributeString("message", $"Unresolved dependency found in {origin.Path}");
      //    foreach (var item in duplicate.Duplicates)
      //      writer.WriteElementString("path", item.Path);
      //  });
      //}
      //  )
      //  exceptions.Add(new InvalidOperationException($"Unresolved dependency found in {line.Reference.Origin?.Type} {line.Reference.Origin?.KeyedName} on the item {line.Reference.Type} {line.Reference.Unique}"));

      //foreach (var exception in exceptions)
      //{
      //  console.WriteLine(exception.Message);
      //  foreach (var dataKey in exception.Data.Keys)
      //    console.WriteLine($"  {dataKey} = {exception.Data[dataKey]}");
      //}
    }
  }
}
