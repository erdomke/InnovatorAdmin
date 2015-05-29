using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Aras.Tools.InnovatorAdmin.Controls
{
  public partial class ImportProgress : UserControl, IWizardStep, IProgessStep
  {
    private ImportProcessor _import;
    private IWizard _wizard;
    private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
    private bool _unlinked = false;

    public Action<ImportProcessor> MethodInvoke { get; set; }
    
    public ImportProgress()
    {
      InitializeComponent();
      
      _timer.Interval = 50;
      _timer.Tick += _timer_Tick;
      progBar.Maximum = 1000000;
    }

    private void UnLink()
    {
      if (!_unlinked)
      {
        _unlinked = true;
        _import.ActionComplete -= _progress_ActionComplete;
        _import.ProgressChanged -= _progress_ProgressChanged;
      }
    }

    void _timer_Tick(object sender, EventArgs e)
    {
      UnLink();
      _wizard.GoToStep(new ImportComplete());
      _timer.Enabled = false;
    }

    void _progress_ActionComplete(object sender, ActionCompleteEventArgs e)
    {
      this.UiThreadInvoke(() =>
      {
        if (e.Exception == null)
        {
          _timer.Enabled = true;
        }
        else
        {
          MessageBox.Show(e.Exception.Message);
        }
      });
    }

    void _progress_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if (this.Parent != null)
      {
        this.UiThreadInvoke(() =>
        {
          progBar.Value = e.Progress;
          lblMessage.Text = e.Message ?? lblMessage.Text;
          if (_import.HasErrors) txtMessage.Text = _import.GetLog();
        });
      }
    }

    public void Configure(IWizard wizard)
    {
      _wizard = wizard;
      _wizard.NextEnabled = false;
      _import = _wizard.ImportProcessor;
      _import.ActionComplete += _progress_ActionComplete;
      _import.ProgressChanged += _progress_ProgressChanged;
      btnCancel.Visible = _import is ICancelableProgressReporter;
      btnCancel.Enabled = true;
      btnCancel.Text = "Cancel";

      var thread = new Thread(o => this.MethodInvoke(_import));
      thread.Start();

      countWorker.RunWorkerAsync();
    }

    public void GoNext()
    {
      throw new NotImplementedException();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      try
      {
        ((ICancelableProgressReporter)_import).Cancel = true;
        btnCancel.Enabled = false;
        btnCancel.Text = "Canceling...";
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void countWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      while (_import.CurrentExtractor == null) Thread.Sleep(20);
      e.Result = _import.CurrentExtractor.GetTotalCount();
    }

    private void countWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      progBar.Maximum = (int)e.Result;
    }
  }
}
