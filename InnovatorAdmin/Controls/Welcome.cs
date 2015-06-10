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

    private void btnImportData_Click(object sender, EventArgs e)
    {
      try
      {
        IDataExtractor result = null;

        using (var dialog = new Dialog.ImportSelectDialog())
        {
          dialog.Filter = "File / Folders to Import|*.*|Data File|*.csv;*.xslx";
          dialog.Multiselect = true;
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            if (dialog.FileName == Utils.GetAppFilePath(AppFileType.ImportExtractor))
            {
              result = DataExtractorFactory.Deserialize(File.ReadAllText(dialog.FileName));
            }
            else if (dialog.FilterIndex == 1 || Directory.Exists(dialog.FileName))
            {
              result = DataExtractorFactory.Get(dialog.FileNames, ImportType.Files);
            }
            else
            {
              throw new NotSupportedException();
            }
          }
        }

        if (result != null)
        {
          var mapping = new ImportMapping();
          mapping.Extractor = result;

          var connSelect = new ConnectionSelection();
          connSelect.MultiSelect = false;
          connSelect.GoNextAction = () => _wizard.GoToStep(mapping);
          _wizard.GoToStep(connSelect);
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
