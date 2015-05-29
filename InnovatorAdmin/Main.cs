using Aras.Tools.InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;

namespace Aras.Tools.InnovatorAdmin
{
  public partial class Main : Form, IWizard
  {
    private IArasConnection _conn;
    private Stack<IWizardStep> _history = new Stack<IWizardStep>();
    private ExportProcessor _export;
    private ImportProcessor _import;
    private InstallProcessor _install;

    public IArasConnection Connection 
    { 
      get { return _conn; }
      set 
      { 
        _conn = value; 
        if (_conn != null)
        {
          if (_import != null)
          {
            _import.ActionComplete -= _import_ActionComplete;
            _import.ProgressChanged -= _import_ProgressChanged;
          } 

          _export = new ExportProcessor(_conn);
          _import = new ImportProcessor(_conn);
          _install = new InstallProcessor(_conn);

          _import.ActionComplete += _import_ActionComplete;
          _import.ProgressChanged += _import_ProgressChanged;
        }
      }
    }
    public IEnumerable<ConnectionData> ConnectionInfo { get; set; }
    public ExportProcessor ExportProcessor { get { return _export; } }
    public ImportProcessor ImportProcessor { get { return _import; } }
    public InstallProcessor InstallProcessor { get { return _install; } }
    public InstallScript InstallScript { get; set; }

    public string Message
    {
      get { return lblMessage.Text; }
      set { lblMessage.Text = value; }
    }
    public bool NextEnabled
    {
      get { return btnNext.Enabled; }
      set { btnNext.Enabled = value; }
    }
    public string NextLabel
    {
      get { return btnNext.Text; }
      set { btnNext.Text = value; }
    }

    public Main()
    {
      InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);

      GoToStep(new Controls.Welcome());
    }

    private DateTime _lastFileWrite = DateTime.Now;

    void _import_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      if ((DateTime.Now - _lastFileWrite).TotalMinutes > 2.0)
      {
        File.WriteAllText(Utils.GetAppFilePath(AppFileType.ImportExtractor), _import.CurrentExtractor.Serialize());
        File.WriteAllText(Utils.GetAppFilePath(AppFileType.ImportLog), _import.GetLog());
        _lastFileWrite = DateTime.Now;
      }
    }

    void _import_ActionComplete(object sender, ActionCompleteEventArgs e)
    {
      File.WriteAllText(Utils.GetAppFilePath(AppFileType.ImportExtractor), _import.CurrentExtractor.Serialize());
      File.WriteAllText(Utils.GetAppFilePath(AppFileType.ImportLog), _import.GetLog());
    }

    public void GoToStep(IWizardStep step)
    {
      if (step is Controls.Welcome) _history.Clear();
      var ctrl = step as Control;
      if (ctrl == null) throw new ArgumentException("Each step must be a control.");

      var curr = tblLayout.GetControlFromPosition(0, 3);
      if (curr != null) tblLayout.Controls.Remove(curr);

      ctrl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
      tblLayout.Controls.Add(ctrl, 0, 3);
      tblLayout.SetColumnSpan(ctrl, 4);
      this.Message = "";
      this.NextLabel = "&Next";
      step.Configure(this);

      btnPrevious.Enabled = _history.Any();
      _history.Push(step);
    }
    
    private void btnClose_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void btnPrevious_Click(object sender, EventArgs e)
    {
      try
      {
        _history.Pop();             // remove the current
        var prevStep = _history.Pop();
        while (prevStep is Controls.IProgessStep)
        {
          ((IDisposable)prevStep).Dispose();
          prevStep = _history.Pop();
        }
        GoToStep(prevStep);         // go to the previous
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void btnNext_Click(object sender, EventArgs e)
    {
      try
      {
        _history.Peek().GoNext();
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private void picHome_Click(object sender, EventArgs e)
    {
      try
      {
        GoToStep(new Controls.Welcome());
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
