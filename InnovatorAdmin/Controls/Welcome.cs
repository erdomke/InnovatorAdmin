using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace InnovatorAdmin.Controls
{
  public partial class Welcome : UserControl, IWizardStep
  {
    private IWizard _wizard;

    public Welcome()
    {
      InitializeComponent();
    }

    public void Configure(IWizard wizard)
    {
      wizard.NextEnabled = false;
      _wizard = wizard;
      _wizard.Message = "Would you like to install an existing package, or create a new one?";
    }

    public void GoNext()
    {
      throw new NotSupportedException();
    }

    private void btnCreate_Click(object sender, EventArgs e)
    {
      try
      {
        var connSelect = new ConnectionSelection();
        connSelect.MultiSelect = false;
        connSelect.GoNextAction = () => _wizard.GoToStep(new ExportSelect());
        _wizard.GoToStep(connSelect);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnInstall_Click(object sender, EventArgs e)
    {
      try
      {
        _wizard.GoToStep(new InstallSource());
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnAmlStudio_Click(object sender, EventArgs e)
    {
      try
      {
        new EditorWindow().Show();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnScripts_Click(object sender, EventArgs e)
    {
      try
      {
        var script = Scripts.ScriptManager.Instance.PromptForScript(this
          , this.RectangleToScreen(btnScripts.Bounds));
        if (script != null)
          new Scripts.ScriptWindow().WithScript(script).Show();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
