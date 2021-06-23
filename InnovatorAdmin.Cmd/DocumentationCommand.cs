using CommandLine;
using InnovatorAdmin.Documentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  [Verb("doc", HelpText = "Document a package from Aras Innovator")]
  public class DocumentationCommand
  {
    [Option('f', "inputfile", HelpText = @"Path to package file containing the items to document. Can be a search pattern: `C:\Directory\**\*.innpkg`", Required = true)]
    public string InputFile { get; set; }

    [Option('o', "outputfile", HelpText = "Path to the file to output", Required = true)]
    public string Output { get; set; }

    [Option("multiple", HelpText = "Whether the output should prefer multiple files")]
    public bool MultipleFiles { get; set; }

    public Task<int> Execute()
    {
      return ConsoleTask.ExecuteAsync(this, async (console) =>
      {
        foreach (var file in GetMatchingFiles(InputFile))
        {
          console.Write("Generating doc for ");
          console.WriteLine(file);

          var metadata = PackageMetadataProvider.FromFile(file);
          var outputPath = Output.Replace("*", CleanFileName(metadata.Title));
          var writer = new DocumentationWriter();
          var extension = Path.GetExtension(outputPath).ToUpperInvariant().TrimStart('.');
          switch (extension)
          {
            case "PUML":
            case "TXT":
              writer.Format = DiagramFormat.PlantUml;
              writer.Output = DocumentOutput.Diagram;
              break;
            case "SVG":
            case "PNG":
              writer.Format = (DiagramFormat)Enum.Parse(typeof(DiagramFormat), extension, true);
              writer.Output = DocumentOutput.Diagram;
              break;
            case "MD":
              writer.Format = DiagramFormat.PlantUml;
              writer.Output = DocumentOutput.Markdown;
              break;
          }

          try
          {
            using (var stream = File.OpenWrite(outputPath))
              await writer.WriteAsync(metadata, stream);
          }
          catch (Exception ex)
          {
            console.WriteLine("Error documenting " + file);
            console.WriteLine(ex.ToString());
          }
        }
      });
    }

    /// <summary>
    /// Removes invalid characters from the path
    /// </summary>
    public static string CleanFileName(string path)
    {
      var invalidChars = System.IO.Path.GetInvalidFileNameChars();
      Array.Sort(invalidChars);
      var builder = new System.Text.StringBuilder(path.Length);
      for (int i = 0; i < path.Length; i++)
      {
        if (Array.BinarySearch(invalidChars, path[i]) < 0 && path[i] != '/')
        {
          builder.Append(path[i]);
        }
      }
      return builder.ToString();
    }

    private IEnumerable<string> GetMatchingFiles(string pattern)
    {
      if (File.Exists(pattern))
        return new[] { pattern };

      var directory = Path.GetDirectoryName(pattern);
      var search = Path.GetFileName(pattern);
      var option = SearchOption.TopDirectoryOnly;
      if (Path.GetFileName(directory) == "**")
      {
        option = SearchOption.AllDirectories;
        directory = Path.GetDirectoryName(directory);
      }
      return Directory.GetFiles(directory, search, option);
    }
  }
}
