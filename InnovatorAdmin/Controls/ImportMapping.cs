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
  public partial class ImportMapping : UserControl, IWizardStep
  {
    private const int PageCount = 50;

    private int _extractorCount = -1;
    private IWizard _wizard;
    private DataTableWriter _tableWriter = new DataTableWriter();
    private XmlDataWriter _xmlWriter = new XmlDataWriter();

    public IDataExtractor Extractor { get; set; }

    public ImportMapping()
    {
      InitializeComponent();

      xsltEditor.Helper = new Editor.ImportXsltHelper();

      string xslt = null;
      if (File.Exists(Utils.GetAppFilePath(AppFileType.XsltAutoSave))) 
        xslt = File.ReadAllText(Utils.GetAppFilePath(AppFileType.XsltAutoSave));
      if (string.IsNullOrWhiteSpace(xslt)) 
        xslt = Properties.Resources.BaseImportXslt;
      xsltEditor.Text = xslt;
    }

    public void Configure(IWizard wizard)
    {
      _wizard = wizard;
      _wizard.NextEnabled = true;
      _wizard.NextLabel = "Import";
      _wizard.Message = "Configure how the data should be imported.";
      ConfigureUi((int)nudBatchSize.Value);
    }

    public void GoNext()
    {
      AutoSaveXslt();
      var xslt = xsltEditor.Text;
      var prog = new ImportProgress();
      prog.MethodInvoke = i =>
      {
        i.ProcessImport(this.Extractor, xslt, (int)nudBatchSize.Value);
      };
      _wizard.GoToStep(prog);
    }

    private void ResetPreview(int minCount)
    {
      this.Extractor.Reset();
      _tableWriter.Reset();
      _xmlWriter.Reset();
      gridPreview.DataSource = null;
      ConfigureUi(Math.Max(minCount, (int)nudBatchSize.Value));
    }
    private void ConfigureUi(int count = PageCount)
    {
      this.Extractor.Write(count, _tableWriter, _xmlWriter);
      if (gridPreview.DataSource == null) gridPreview.DataSource = _tableWriter.Table;
      xmlEditor.Text = _xmlWriter.ToString();
      btnLoadMore.Enabled = !this.Extractor.AtEnd;
      if (_extractorCount < 0 && !countWorker.IsBusy) countWorker.RunWorkerAsync();
      UpdateCountLabel();
    }
    private void UpdateCountLabel()
    {
      lblCount.Text = string.Format("Showing {0} of {1}", this.Extractor.NumProcessed, (_extractorCount >= 0 ? _extractorCount.ToString() : "?"));
    }

    private void chkChecksum_CheckedChanged(object sender, EventArgs e)
    {
      var fileExtract = this.Extractor as FileSysExtractor;
      if (fileExtract != null)
      {
        fileExtract.IncludeChecksum = chkChecksum.Checked;
        ResetPreview(gridPreview.RowCount);
      }
    }

    private void chkFileSize_CheckedChanged(object sender, EventArgs e)
    {
      var fileExtract = this.Extractor as FileSysExtractor;
      if (fileExtract != null)
      {
        fileExtract.IncludeSize = chkFileSize.Checked;
        ResetPreview(gridPreview.RowCount);
      }
    }

    private void btnLoadMore_Click(object sender, EventArgs e)
    {
      ConfigureUi((int)nudBatchSize.Value);
    }

    private void RunTest(string query)
    {
      outputEditor.Text = ArasXsltExtensions.Transform(query, _xmlWriter.ToString(), _wizard.Connection);
    }

    private void xsltEditor_RunRequested(object sender, Editor.RunRequestedEventArgs e)
    {
      try
      {
        RunTest(e.Query);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void countWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      e.Result = this.Extractor.GetTotalCount();
    }

    private void countWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      _extractorCount = (int)e.Result;
      UpdateCountLabel();
    }

    private void AutoSaveXslt()
    {
      File.WriteAllText(Utils.GetAppFilePath(AppFileType.XsltAutoSave), xsltEditor.Text);
    }

    private void timerAutoSave_Tick(object sender, EventArgs e)
    {
      try
      {
        AutoSaveXslt();
      }
      catch (Exception) { }
    }

    private void btnOpen_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new OpenFileDialog())
        {
          dialog.CheckPathExists = true;
          dialog.Filter = "XML Stylesheets (*.xslt, *.xsl)|*.xslt;*.xsl";
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            xsltEditor.Text = File.ReadAllText(dialog.FileName);
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
      try
      {
        using (var dialog = new SaveFileDialog())
        {
          dialog.Filter = "XML Stylesheets (*.xslt)|*.xslt";
          if (dialog.ShowDialog() == DialogResult.OK)
          {
            File.WriteAllText(dialog.FileName, xsltEditor.Text);
          }
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnTest_Click(object sender, EventArgs e)
    {
      try
      {
        RunTest(xsltEditor.Text);
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
