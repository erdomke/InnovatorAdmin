using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Windows.Forms.Integration;
using ICSharpCode.AvalonEdit.Document;
using System.Threading.Tasks;

namespace InnovatorAdmin.Controls
{
  public partial class CompareSelect : UserControl, IWizardStep
  {
    private IWizard _wizard;

    public CompareSelect()
    {
      InitializeComponent();
    }

    public void Configure(IWizard wizard)
    {
      _wizard = wizard;
      wizard.NextLabel = "Compare";
      wizard.NextEnabled = true;
    }

    public void GoNext()
    {
      var compare = new Compare();
      compare.BaseInstall = InnovatorPackage.Load(leftPath.Text).Read();
      _wizard.InstallScript = InnovatorPackage.Load(rightPath.Text).Read();
      _wizard.GoToStep(compare);
    }
  }
}
