using CommandLine;
using CommandLine.Text;
using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace InnovatorAdmin.Cmd
{
  [Verb("export", HelpText = "Export a solution from Aras Innovator")]
  internal class ExportCommand : SharedOptions
  {
    [Option("title", HelpText = "Title of the package")]
    public string Title { get; set; }

    [Option("author", HelpText = "Author name to include with the package metadata")]
    public string Author { get; set; }

    [Option("website", HelpText = "Website URL to include with the package metadata")]
    public string Website { get; set; }

    [Option("description", HelpText = "Description to include with the package metadata")]
    public string Description { get; set; }

    [Option('o', "outputfile", HelpText = "Path to the file to output", Required = true)]
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
        yield return new Example("Example", new UnParserSettings() { PreferShortName = true }, new ExportCommand()
        {
          Url = "http://localhost/InnovatorServer/",
          Database = "InnovatorSolutions",
          Username = "admin",
          Password = "innovator",
          Output = @"C:\Output\Path\TestPackage.innpkg",
          MultipleDirectories = true,
          CleanOutput = true
        });
      }
    }

    public Task<int> Execute()
    {
      return ConsoleTask.ExecuteAsync(this, async (console) =>
      {
        console.WriteLine("Connecting to innovator...");
        var conn = await this.GetConnection().ConfigureAwait(false);
        var processor = new ExportProcessor(conn);

        var refsToExport = default(List<ItemReference>);
        var checkDependencies = true;

        console.Write("Identifying items to export... ");
        if (this.InputFile?.EndsWith(".innpkg", StringComparison.OrdinalIgnoreCase) == true
          || this.InputFile?.EndsWith(".mf", StringComparison.OrdinalIgnoreCase) == true)
        {
          var exportScript = InnovatorPackage.Load(this.InputFile).Read();
          refsToExport = exportScript.Lines
            .Where(l => l.Type == InstallType.Create)
            .Select(l => l.Reference)
            .Distinct()
            .ToList();
        }
        else
        {
          var exportQuery = XElement.Parse("<AML><Item type='*' /></AML>");
          if (!string.IsNullOrEmpty(this.InputFile))
            exportQuery = XElement.Load(this.InputFile);

          var firstItem = exportQuery.XPathSelectElement("//Item[1]");
          if (firstItem == null)
            throw new Exception("No item nodes could be found");

          var items = default(IEnumerable<XElement>);
          if (firstItem.Parent == null)
            items = new[] { firstItem };
          else
            items = firstItem.Parent.Elements("Item");

          var version = await conn.FetchVersion(true).ConfigureAwait(false);
          var types = ExportAllType.Types.Where(t => t.Applies(version)).ToList();
          var queries = GetQueryies(items, types).ToList();
          checkDependencies = items.All(e => e.Attribute("type")?.Value != "*");

          using (var prog = console.Progress())
          {
            var toExport = await SharedUtils.TaskPool(30, (l, m) => prog.Report(l / 100.0), queries
              .Select(q =>
              {
                var aml = new XElement(q);
                var levels = aml.Attribute("levels");
                if (levels != null)
                  levels.Remove();
                return (Func<Task<QueryAndResult>>)(() => conn.ApplyAsync(aml, true, false)
                  .ToTask()
                  .ContinueWith(t => new QueryAndResult()
                  {
                    Query = q,
                    Result = t.Result
                  }));
              })
              .ToArray());
            refsToExport = toExport.SelectMany(r =>
              {
                var refs = r.Result.Items()
                  .Select(i => ItemReference.FromFullItem(i, true))
                  .ToList();
                var levels = (int?)r.Query.Attribute("levels");
                if (levels.HasValue)
                {
                  foreach (var iRef in refs)
                    iRef.Levels = levels.Value;
                }
                return refs;
              })
              .Distinct()
              .ToList();
          }
        }
        console.WriteLine("Done.");

        var script = new InstallScript
        {
          ExportUri = new Uri(Url),
          ExportDb = Database,
          Lines = Enumerable.Empty<InstallItem>(),
          Title = Title ?? System.IO.Path.GetFileNameWithoutExtension(Output),
          Creator = Author ?? Username,
          Website = string.IsNullOrEmpty(Website) ? null : new Uri(Website),
          Description = Description,
          Created = DateTime.Now,
          Modified = DateTime.Now
        };

        console.Write("Exporting metadata... ");
        using (var prog = console.Progress())
        {
          processor.ProgressChanged += (s, e) => prog.Report(e.Progress / 100.0);
          processor.ActionComplete += (s, e) =>
          {
            if (e.Exception != null)
              throw new AggregateException(e.Exception);
          };
          await processor.Export(script, refsToExport, checkDependencies);
        }
        console.WriteLine("Done.");

        WritePackage(console, script, Output, MultipleDirectories, CleanOutput);
      });
    }

    private class QueryAndResult
    {
      public XElement Query { get; set; }
      public IReadOnlyResult Result { get; set; }
    }

    private IEnumerable<XElement> GetQueryies(IEnumerable<XElement> items, IEnumerable<ExportAllType> defaultTypes)
    {
      foreach (var item in items)
      {
        if (item.Attribute("type")?.Value == "*")
        {
          foreach (var type in defaultTypes)
            yield return XElement.Parse(type.ToString());
        }
        else
        {
          if (string.IsNullOrEmpty(item.Attribute("action")?.Value))
            item.SetAttributeValue("action", "get");
          if (string.IsNullOrEmpty(item.Attribute("type")?.Value)
            && string.IsNullOrEmpty(item.Attribute("typeId")?.Value))
            throw new NotSupportedException("Neither type nor typeId is specified");
          item.SetAttributeValue("select", "config_id");
          yield return item;
        }
      }
    }
  }
}
