using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace InnovatorAdmin.Powershell
{
  [Cmdlet(VerbsCommon.Get, "Connection")]
  [OutputType(typeof(SavedConnection))]
  public class GetConnection : Cmdlet
  {
    [Parameter]
    public string FilePath { get; set; }

    [Parameter(ValueFromPipeline = true)]
    public string Name { get; set; }

    protected override void ProcessRecord()
    {
      IEnumerable<ConnectionPreferences> prefs = string.IsNullOrEmpty(FilePath)
        ? SavedConnections.Load()
        : SavedConnections.Load(FilePath);
      if (!string.IsNullOrEmpty(Name))
      {
        prefs = prefs.Where(p => p.Name.IndexOf(Name, StringComparison.OrdinalIgnoreCase) >= 0);
      }

      foreach (var conn in prefs.Select(p => new SavedConnection(p)))
      {
        WriteObject(conn);
      }
    }
  }
}
