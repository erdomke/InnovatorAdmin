using Innovator.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace InnovatorAdmin
{
  public interface IWizard
  {
    IAsyncConnection Connection { get; set; }
    IEnumerable<Connections.ConnectionData> ConnectionInfo { get; set; }
    ExportProcessor ExportProcessor { get; }
    void GoToStep(IWizardStep step);
    //ImportProcessor ImportProcessor { get; }
    InstallProcessor InstallProcessor { get; }
    InstallScript InstallScript { get; set; }
    bool NextEnabled { get; set; }
    string NextLabel { get; set; }
    string Message { get; set; }
  }
}
