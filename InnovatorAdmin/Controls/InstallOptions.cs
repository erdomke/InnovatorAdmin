using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  public partial class InstallOptions : UserControl, IWizardStep
  {
    IWizard _wizard;

    public InstallOptions()
    {
      InitializeComponent();
    }

    public void Configure(IWizard wizard)
    {
      _wizard = wizard;
      _wizard.Message = "Configure installation";
      _wizard.NextEnabled = true;
    }

    public void GoNext()
    {
      var connStep = new ConnectionSelection();
      connStep.MultiSelect = true;
      connStep.GoNextAction = () =>
      {
        _wizard.GoToStep(new InstallProgress());
      };
      _wizard.GoToStep(connStep);
      _wizard.NextLabel = "&Install";
    }
  }
}
