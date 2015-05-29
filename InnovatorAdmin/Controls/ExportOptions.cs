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

      txtName.Focus();
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
            dialog.Filter = "Innovator Package (Single file)|*.innpkg|Innovator Package (Multiple files: for development)|*.innpkg|Manifest (Backwards compatible)|*.mf";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
              switch (dialog.FilterIndex)
              {
                case 2:
                  var pkgFolder = new InnovatorPackageFolder(dialog.FileName);
                  if (!_wizard.InstallScript.Created.HasValue) _wizard.InstallScript.Created = DateTime.Now;
                  _wizard.InstallScript.Modified = DateTime.Now;
                  pkgFolder.Write(_wizard.InstallScript);
                  break;
                case 3:
                  var manifest = new ManifestFolder(dialog.FileName);
                  manifest.Write(_wizard.InstallScript);
                  break;
                default:
                  using (var pkgFile = new InnovatorPackageFile(dialog.FileName))
                  {
                    if (!_wizard.InstallScript.Created.HasValue) _wizard.InstallScript.Created = DateTime.Now;
                    _wizard.InstallScript.Modified = DateTime.Now;
                    pkgFile.Write(_wizard.InstallScript);
                  }
                  break;
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
          var dbPackage = new DatabasePackage(_wizard.Connection);
          dbPackage.Write(_wizard.InstallScript);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnCompare_Click(object sender, EventArgs e)
    {
      try
      {
        var currInstall = _wizard.InstallScript;
        var refs = (from i in _wizard.InstallScript.Lines
                    where i.Reference != null && i.Type == InstallType.Create
                    select i.Reference);

        var connStep = new ConnectionSelection();
        connStep.MultiSelect = false;
        connStep.GoNextAction = () =>
        {
          var prog = new ProgressStep<ExportProcessor>(_wizard.ExportProcessor);
          prog.MethodInvoke = p =>
          {
            _wizard.InstallScript = new InstallScript();
            _wizard.InstallScript.ExportUri = new Uri(_wizard.ConnectionInfo.First().Url);
            _wizard.InstallScript.ExportDb = _wizard.ConnectionInfo.First().Database;
            _wizard.InstallScript.Lines = Enumerable.Empty<InstallItem>();
            p.Export(_wizard.InstallScript, refs);
          };
          prog.GoNextAction = () => {
            var compare = new Compare();
            compare.BaseInstall = currInstall;
            _wizard.GoToStep(compare);
          };

          _wizard.GoToStep(prog);
        };
        _wizard.GoToStep(connStep);
        _wizard.NextLabel = "&Export Compare";
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
