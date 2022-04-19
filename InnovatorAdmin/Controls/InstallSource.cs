using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin.Controls
{
  public partial class InstallSource : UserControl, IWizardStep
  {
    private IWizard _wizard;
    private Func<Task<InstallScript>> _createInstallScript;

    public InstallSource()
    {
      InitializeComponent();
    }

    private void btnInnovatorPackage_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new OpenFileDialog())
        {
          dialog.Filter = "Innovator Package (.innpkg)|*.innpkg|Manifest (.mf)|*.mf";
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            if (Path.GetExtension(dialog.FileName) == ".innpkg")
            {
              using (var pkg = InnovatorPackage.Load(dialog.FileName))
              {
                _wizard.InstallScript = pkg.Read();
              }
              SetMetadata();
            }
            else
            {
              var pkg = new ManifestFolder(dialog.FileName);
              string title;
              var doc = pkg.Read(out title);
              _createInstallScript = () => _wizard.InstallProcessor.ConvertManifestXml(doc, title);
              lblName.Text = title;
              _wizard.NextEnabled = true;
            }
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void SetMetadata()
    {
      lblAuthor.Text = _wizard.InstallScript.Creator;
      lblName.Text = _wizard.InstallScript.Title;
      lblWebsite.Text = (_wizard.InstallScript.Website == null ? "" : _wizard.InstallScript.Website.ToString());
      txtDescription.Text = _wizard.InstallScript.Description;
      _wizard.NextEnabled = true;
    }

    public void Configure(IWizard wizard)
    {
      _wizard = wizard;
      _wizard.Message = "Select a package to install";
      _wizard.NextEnabled = false;
    }

    public void GoNext()
    {
      var connStep = new ConnectionSelection();
      connStep.MultiSelect = true;
      connStep.GoNextAction = async () =>
      {
        if (_createInstallScript != null)
          _wizard.InstallScript = await _createInstallScript.Invoke();
        _wizard.GoToStep(new InstallProgress());
      };
      _wizard.GoToStep(connStep);
      _wizard.NextLabel = "&Install";
    }

    private void lblWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
      try
      {
        if (!string.IsNullOrEmpty(lblWebsite.Text)) Process.Start(lblWebsite.Text);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }

    }
  }
}
