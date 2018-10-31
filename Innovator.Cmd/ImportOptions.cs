using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Cmd
{
  [Verb("import", HelpText = "Import a solution from Aras Innovator")]
  class ImportOptions : SharedOptions
  {
  }
}
