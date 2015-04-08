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
      throw new NotImplementedException();
    }

    private void btnResetServerCache_Click(object sender, EventArgs e)
    {
      string msg;
      using (var rsc = new ConnectionEditorForm()) {
        if (rsc.ShowDialog(this.ParentForm) == DialogResult.OK)
        {
          foreach (var conn in rsc.SelectedConnections)
          {
            var inn = ConnectionEditor.Login(conn, out msg);
            if (inn == null)
            {
              MessageBox.Show(msg);
            }
            else
            {
              var output = new XmlDocument();
              output.AppendChild(output.CreateElement("Item"));
              var inDoc = new XmlDocument();
              inDoc.AppendChild(inDoc.CreateElement("Item"));
              inn.getConnection().CallAction("ResetServerCache", inDoc, output);
            }

          }
        }
      }

    }
  }
}
