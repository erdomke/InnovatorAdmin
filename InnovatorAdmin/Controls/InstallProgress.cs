using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace InnovatorAdmin.Controls
{
  public partial class InstallProgress : UserControl, IWizardStep, IProgessStep
  {
    private IWizard _wizard;
    private System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
    private Exception _ex;
    private List<Connections.ConnectionData> _connections;
    private System.Windows.Forms.Timer _clock = new System.Windows.Forms.Timer();
    private DateTime _start = DateTime.UtcNow;

    public InstallProgress()
    {
      InitializeComponent();

      _timer.Interval = 50;
      _timer.Tick += _timer_Tick;

      _clock.Interval = 250;
      _clock.Tick += _clock_Tick;
      _clock.Enabled = true;

      GlobalProgress.Instance.Start();
    }

    void _clock_Tick(object sender, EventArgs e)
    {
      lblTime.Text = (DateTime.UtcNow - _start).ToString(@"hh\:mm\:ss");
    }

    void _timer_Tick(object sender, EventArgs e)
    {
      try
      {
        GlobalProgress.Instance.Stop();
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
      Link();

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
          UnLink();
          _wizard.Connection = conn;
          Link();
        }

        _ex = null;
        var thread = new Thread(o =>
        {
          _wizard.InstallProcessor.Initialize(_wizard.InstallScript)
            .ContinueWith(t => _wizard.InstallProcessor.Install());
        });
        thread.Start();
        return true;
      }
      return false;
    }

    private void Link()
    {
      var xmlPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Innovator Admin", "InstallLog_" + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".xml");
      Directory.CreateDirectory(Path.GetDirectoryName(xmlPath));
      _wizard.InstallProcessor.LogWriter = XmlWriter.Create(xmlPath, new XmlWriterSettings()
      {
        Indent = true,
        IndentChars = "  ",
        ConformanceLevel = ConformanceLevel.Fragment,
        OmitXmlDeclaration = true,
      });
      _wizard.InstallProcessor.ActionComplete += InstallProcessor_ActionComplete;
      _wizard.InstallProcessor.ErrorRaised += InstallProcessor_ErrorRaised;
      _wizard.InstallProcessor.ProgressChanged += InstallProcessor_ProgressChanged;
    }

    private void UnLink()
    {
      if (this.IsDisposed)
        return;
      _wizard.InstallProcessor.LogWriter?.Dispose();
      _wizard.InstallProcessor.LogWriter = null;
      _wizard.InstallProcessor.ActionComplete -= InstallProcessor_ActionComplete;
      _wizard.InstallProcessor.ErrorRaised -= InstallProcessor_ErrorRaised;
      _wizard.InstallProcessor.ProgressChanged -= InstallProcessor_ProgressChanged;
    }

    void InstallProcessor_ActionComplete(object sender, ActionCompleteEventArgs e)
    {
      this.UiThreadInvoke(() =>
      {
        _wizard.InstallProcessor.LogWriter.Dispose();
        _wizard.InstallProcessor.LogWriter = null;
        _timer.Enabled = true;
        if (e.Exception != null)
          Utils.HandleError(e.Exception);
      });
    }

    void InstallProcessor_ErrorRaised(object sender, RecoverableErrorEventArgs e)
    {
      this.UiThreadInvoke(() =>
      {
        GlobalProgress.Instance.Error();
        ErrorWindow.HandleEvent(e, _wizard.Connection);
        GlobalProgress.Instance.Start();
      });
    }

    void InstallProcessor_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      this.UiThreadInvoke(() =>
      {
        progBar.Value = e.Progress;
        lblMessage.Text = _wizard.Connection.Database + ": " + (e.Message ?? lblMessage.Text);
        GlobalProgress.Instance.Value(e.Progress);
      });
    }

    public void GoNext()
    {
      throw new NotSupportedException("You cannot manually go next");
    }
  }
}
