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
  [Verb("index", HelpText = "Create an index of a package file")]
  internal class CreateIndexCommand
  {
    [Option('f', "inputfile", HelpText = "Path to package file containing the items to import/export")]
    public string InputFile { get; set; }

    public int Execute()
    {
      return ConsoleTask.Execute(this, console =>
      {
        if (!Directory.Exists(InputFile))
          throw new InvalidOperationException();


      });
    }
  }
}
