using CommandLine;
using CommandLine.Text;
using Innovator.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace InnovatorAdmin.Cmd
{
  [Verb("export", HelpText = "Export a solution from Aras Innovator")]
  internal class ExportCommand : SharedOptions, ICommand
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

    public async Task<int> Execute(ILogger logger)
    {
      var conn = await this.GetConnection().ConfigureAwait(false);
      var processor = new ExportProcessor(conn, logger);

      var refsToExport = default(List<ItemReference>);
      var checkDependencies = true;
      var progress = new ProgressLogger(logger);

      using (SharedUtils.StartActivity("Identifying items to export"))
      {

        if (this.InputFile?.EndsWith(".innpkg", StringComparison.OrdinalIgnoreCase) == true
          || this.InputFile?.EndsWith(".mf", StringComparison.OrdinalIgnoreCase) == true)
        {
          var exportScript = Package.Create(this.InputFile).Single().Read();
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
          var types = ExportAllType.Types
            .Where(t => t.Applies(version))
            .ToList();
          var allTypes = (await conn.ApplyAsync($@"<Item type='ItemType' action='get' select='name' />", true, true).ConfigureAwait(false))
            .Items()
            .ToList();
          var exitingTypesByName = new HashSet<string>(allTypes
            .Select(i => i.Property("name").AsString(null)), StringComparer.OrdinalIgnoreCase);
          var exitingTypesById = new HashSet<string>(allTypes
            .Select(i => i.Id()), StringComparer.OrdinalIgnoreCase);
          types = types.Where(t => exitingTypesByName.Contains(t.Name)).ToList();

          var queries = GetQueryies(items, types);
          var cantExport = queries
            .Where(i => !exitingTypesByName.Contains((string)i.Attribute("type") ?? "")
              && !exitingTypesById.Contains((string)i.Attribute("typeId") ?? ""))
            .Select(i => (string)i.Attribute("type") ?? (string)i.Attribute("typeId") ?? "?")
            .ToList();
          if (cantExport.Count > 0)
            logger.LogWarning("The following types could not be exported: {Types}", string.Join(", ", cantExport));

          queries = queries
            .Where(i => exitingTypesByName.Contains((string)i.Attribute("type") ?? "")
              || exitingTypesById.Contains((string)i.Attribute("typeId") ?? ""))
            .ToList();
          checkDependencies = items.All(e => e.Attribute("type")?.Value != "*");

          progress.Reset();
          var toExport = await SharedUtils.TaskPool(30, progress.Report, queries
            .Select(q =>
            {
              var aml = new XElement(q);
              aml.Attribute("levels")?.Remove();
              return (Func<Task<QueryAndResult>>)(async () => {
                var t = await conn.ApplyAsync(aml, true, false).ConfigureAwait(false);
                return new QueryAndResult()
                {
                  Query = q,
                  Result = t
                };
              });
            })
            .ToArray()).ConfigureAwait(false);
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

      progress.Reset();
      processor.ProgressChanged += progress.Report;
      processor.ActionComplete += (s, e) =>
      {
        if (e.Exception != null)
          throw new AggregateException(e.Exception);
      };
      await processor.Export(script, refsToExport, checkDependencies);

      WritePackage(script, Output, MultipleDirectories, CleanOutput);

      return 0;
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
