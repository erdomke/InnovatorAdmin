using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace InnovatorAdmin.Cmd
{
  [Verb("repair", HelpText = "Repair a package file")]
  internal class RepairCommand
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
        var elem = XElement.Load(InputFile);

        if (RemoveMetadata)
        {
          elem.Attribute("created")?.Remove();
          elem.Attribute("creator")?.Remove();
          elem.Attribute("modified")?.Remove();
        }

        var baseDir = Path.GetDirectoryName(InputFile);
        foreach (var path in elem.Elements("Path").ToList())
        {
          var fullPath = Path.Combine(baseDir, path.Attribute("path")?.Value);
          if (!File.Exists(fullPath))
          {
            Console.WriteLine("Removing reference to `{0}`", path.Attribute("path")?.Value);
            path.Remove();
          }
        }

        console.WriteLine("Writing file...");
        using (var writer = XmlWriter.Create(InputFile, new XmlWriterSettings()
        {
          OmitXmlDeclaration = true,
          Indent = true
        }))
        {
          elem.Save(writer);
        }
      });
    }
  }
}
