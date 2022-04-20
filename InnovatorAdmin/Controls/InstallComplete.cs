using Innovator.Client;
using System;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  public partial class InstallComplete : UserControl, IWizardStep
  {
    public InstallComplete()
    {
      InitializeComponent();
    }

    public void Configure(IWizard wizard)
    {
      wizard.NextEnabled = false;
    }

    public void GoNext()
    {
      // Do nothing
    }

    private void btnResetServerCache_Click(object sender, EventArgs e)
    {
      using (var rsc = new ConnectionEditorForm()) {
        if (rsc.ShowDialog(this.ParentForm,
          btnResetServerCache.RectangleToScreen(btnResetServerCache.Bounds)) == DialogResult.OK)
        {
          foreach (var conn in rsc.SelectedConnections)
          {
            var arasConn = conn.ArasLogin();
            arasConn.Process(new Command("<Item/>").WithAction("ResetServerCache"));
          }
        }
      }

    }
  }
}
