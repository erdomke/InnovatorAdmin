using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Aras.Tools.InnovatorAdmin.Controls
{
  public partial class ExportOptions : UserControl, IWizardStep
  {
    private IWizard _wizard;

    public ExportOptions()
    {
      InitializeComponent();
    }

    public void Configure(IWizard wizard)
    {
      _wizard = wizard;
      _wizard.Message = "Specify package metadata (optional) and select and export type.";
      _wizard.NextEnabled = false;

      txtName.Text = _wizard.InstallScript.Title;
      txtAuthor.Text = _wizard.InstallScript.Creator ?? _wizard.ConnectionInfo.First().UserName;
      txtWebsite.Text = (_wizard.InstallScript.Website == null ? "" : _wizard.InstallScript.Website.ToString());
      txtDescription.Text = _wizard.InstallScript.Description;
    }

    private bool FlushMetadata()
    {
      if (string.IsNullOrEmpty(txtName.Text))
      {
        MessageBox.Show("A name must be specified for the package.");
        return false;
      }
      _wizard.InstallScript.Title = txtName.Text;
      _wizard.InstallScript.Creator = txtAuthor.Text;
      Uri website;
      if (Uri.TryCreate(txtWebsite.Text, UriKind.Absolute, out website))
      {
        _wizard.InstallScript.Website = website;
      }
      else
      {
        _wizard.InstallScript.Website = null;
      }
      _wizard.InstallScript.Description = txtDescription.Text;
      return true;
    }

    public void GoNext()
    {
      throw new NotSupportedException();
    }

    private void btnPackageFile_Click(object sender, EventArgs e)
    {
      try
      {
        if (FlushMetadata())
        {
          using (var dialog = new SaveFileDialog())
          {
            if (!string.IsNullOrEmpty(_wizard.InstallScript.Title))
            {
              dialog.FileName = _wizard.InstallScript.Title + ".innpkg";
            }
            dialog.Filter = "Innovator Package|*.innpkg|Manifest|*.mf";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
              if (Path.GetExtension(dialog.FileName) == ".innpkg")
              {
                using (var pkg = new InnovatorPackage(dialog.FileName))
                {
                  if (!_wizard.InstallScript.Created.HasValue) _wizard.InstallScript.Created = DateTime.Now;
                  _wizard.InstallScript.Modified = DateTime.Now;
                  pkg.Write(_wizard.InstallScript);
                }
              }
              else
              {
                var pkg = new ManifestFolder(dialog.FileName);
                pkg.Write(_wizard.InstallScript);
              }
            }
          }
        }
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
        if (FlushMetadata())
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
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnDbPackage_Click(object sender, EventArgs e)
    {
      try
      {
        if (FlushMetadata())
        {
          var dbPackage = new DatabasePackage(_wizard.ApplyAction);
          dbPackage.Write(_wizard.InstallScript);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
