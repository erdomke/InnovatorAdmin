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
  public partial class InstallComplete : UserControl, IWizardStep
  {
    private IWizard _wizard;

    public InstallComplete()
    {
      InitializeComponent();
    }

    public void Configure(IWizard wizard)
    {
      wizard.NextLabel = "Start Over";
      wizard.NextEnabled = true;
      _wizard = wizard;
    }

    public void GoNext()
    {
      _wizard.GoToStep(new Welcome());
    }

    private void btnResetServerCache_Click(object sender, EventArgs e)
    {
      string msg;
      using (var rsc = new ConnectionEditorForm()) {
        if (rsc.ShowDialog(this.ParentForm) == DialogResult.OK)
        {
          foreach (var conn in rsc.SelectedConnections)
          {
            var arasConn = ConnectionEditor.Login(conn, out msg);
            if (arasConn == null)
            {
              MessageBox.Show(msg);
            }
            else
            {
              arasConn.CallAction("ResetServerCache", "<Item/>");
            }

          }
        }
      }

    }
  }
}
