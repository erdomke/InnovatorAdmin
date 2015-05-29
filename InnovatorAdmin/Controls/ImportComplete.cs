using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin.Controls
{
  public partial class ImportComplete : UserControl, IWizardStep
  {
    private IWizard _wizard;

    public ImportComplete()
    {
      InitializeComponent();
    }

    public void Configure(IWizard wizard)
    {
      wizard.NextLabel = "Start Over";
      wizard.NextEnabled = true;
      _wizard = wizard;
      txtMessage.Text = _wizard.ImportProcessor.GetLog();
    }

    public void GoNext()
    {
      _wizard.GoToStep(new Welcome());
    }
  }
}
