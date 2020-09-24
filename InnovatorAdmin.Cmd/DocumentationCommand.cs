using CommandLine;
using InnovatorAdmin.Documentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
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
        switch (Path.GetExtension(outputPath).ToUpperInvariant())
        {
          case ".PUML":
          case ".TXT":
            var diagram = EntityDiagram.FromTypes(metadata.ItemTypes, metadata.Title);
            var uml = new PlantUmlWriter();
            using (var writer = new StreamWriter(outputPath))
              uml.Write(diagram, writer);
            break;
          case ".SVG":
          case ".PNG":
            var diagramImg = EntityDiagram.FromTypes(metadata.ItemTypes, metadata.Title);
            var urlWriter = new PlantUmlUrlWriter()
            {
              Format = Path.GetExtension(outputPath).TrimStart('.').ToLowerInvariant()
            };
            using (var writer = new StringWriter())
            {
              urlWriter.Write(diagramImg, writer);
              var client = new HttpClient();
              var url = writer.ToString();
              try
              {
                using (var readStream = await client.GetStreamAsync(url))
                using (var writeStream = File.OpenWrite(outputPath))
                {
                  await readStream.CopyToAsync(writeStream);
                }
              }
              catch (HttpRequestException ex)
              {
                console.WriteLine(url);
                console.WriteLine(ex.ToString());
              }
            }
            break;
          case ".MD":
            var diagramMd = EntityDiagram.FromTypes(metadata.ItemTypes, metadata.Title);
            var umlMd = new PlantUmlWriter();
            using (var writer = new StreamWriter(outputPath))
            {
              writer.WriteLine("# Diagram");
              writer.WriteLine();
              writer.WriteLine("```plantuml");
              umlMd.Write(diagramMd, writer);
              writer.WriteLine();
              writer.WriteLine("```");
              writer.WriteLine();
              var mdWriter = new MarkdownVisitor(writer);
              foreach (var itemType in metadata.ItemTypes.Where(i => !i.IsUiOnly).OrderBy(i => i.Name))
              {
                mdWriter.Visit(Document.FromItemType(itemType, new DocumentOptions()
                {
                  IncludeCrossReferenceLinks = false,
                  IncludeCoreProperties = false
                }, metadata));
              }
            }
            break;
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
