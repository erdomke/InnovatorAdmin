using Aras.IOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public interface IWizard
  {
    Func<string, XmlNode, XmlNode> ApplyAction { get; }
    IEnumerable<Connections.ConnectionData> ConnectionInfo { get; set; }
    ExportProcessor ExportProcessor { get; }
    void GoToStep(IWizardStep step);
    Innovator Innovator { get; set; }
    InstallProcessor InstallProcessor { get; }
    InstallScript InstallScript { get; set; }
    bool NextEnabled { get; set; }
    string NextLabel { get; set; }
    string Message { get; set; }
  }
}
