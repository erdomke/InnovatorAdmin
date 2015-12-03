using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace InnovatorAdmin.Controls
{
  public partial class InstallProgress : UserControl, IWizardStep, IProgessStep
  {
    private IWizard _wizard;
    private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
    private Exception _ex;
    private bool _unlinked = false;
    private List<Connections.ConnectionData> _connections;

    public InstallProgress()
    {
      InitializeComponent();

      _timer.Interval = 50;
      _timer.Tick += _timer_Tick;
    }

    void _timer_Tick(object sender, EventArgs e)
    {
      try
      {
        if (_ex == null)
        {
          _connections.RemoveAt(0);
          if (!StartInstall(false)) _wizard.GoToStep(new InstallComplete());
        }
        else
        {
          Utils.HandleError(_ex);
        }
        _timer.Enabled = false;
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
    }

    public void Configure(IWizard wizard)
    {
      _wizard = wizard;
      _wizard.NextEnabled = false;
      _wizard.InstallProcessor.ActionComplete += InstallProcessor_ActionComplete;
      _wizard.InstallProcessor.ErrorRaised += InstallProcessor_ErrorRaised;
      _wizard.InstallProcessor.ProgressChanged += InstallProcessor_ProgressChanged;

      _connections = _wizard.ConnectionInfo.ToList();
      StartInstall(true);
    }

    private bool StartInstall(bool isLoggedIn)
    {
      if (_connections.Any())
      {
        if (!isLoggedIn)
        {
          var conn = _connections.First().ArasLogin();
          _wizard.InstallProcessor.ActionComplete -= InstallProcessor_ActionComplete;
          _wizard.InstallProcessor.ErrorRaised -= InstallProcessor_ErrorRaised;
          _wizard.InstallProcessor.ProgressChanged -= InstallProcessor_ProgressChanged;
          _wizard.Connection = conn;
          _wizard.InstallProcessor.ActionComplete += InstallProcessor_ActionComplete;
          _wizard.InstallProcessor.ErrorRaised += InstallProcessor_ErrorRaised;
          _wizard.InstallProcessor.ProgressChanged += InstallProcessor_ProgressChanged;
        }

        _ex = null;
        var thread = new Thread(o =>
        {
          _wizard.InstallProcessor.Initialize(_wizard.InstallScript);
          _wizard.InstallProcessor.Install();
        });
        thread.Start();
        return true;
      }
      return false;
    }

    private void UnLink()
    {
      if (!_unlinked)
      {
        _unlinked = true;
        _wizard.InstallProcessor.ActionComplete -= InstallProcessor_ActionComplete;
        _wizard.InstallProcessor.ErrorRaised -= InstallProcessor_ErrorRaised;
        _wizard.InstallProcessor.ProgressChanged -= InstallProcessor_ProgressChanged;
      }
    }

    void InstallProcessor_ActionComplete(object sender, ActionCompleteEventArgs e)
    {
      this.UiThreadInvoke(() =>
      {
        _timer.Enabled = true;
      });
    }

    void InstallProcessor_ErrorRaised(object sender, RecoverableErrorEventArgs e)
    {
      this.UiThreadInvoke(() =>
      {
        ErrorWindow.HandleEvent(e);
      });
    }

    void InstallProcessor_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      this.UiThreadInvoke(() =>
      {
        progBar.Value = e.Progress;
        lblMessage.Text = _wizard.Connection.Database + ": " + (e.Message ?? lblMessage.Text);
      });
    }

    public void GoNext()
    {
      throw new NotSupportedException();
    }
  }
}
