using Aras.Tools.InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Innovator.Client;

namespace Aras.Tools.InnovatorAdmin
{
  public partial class Main : Form, IWizard
  {
    private IAsyncConnection _conn;
    private Stack<IWizardStep> _history = new Stack<IWizardStep>();
    private ExportProcessor _export;
    //private ImportProcessor _import;
    private InstallProcessor _install;

    public IAsyncConnection Connection
    {
      get { return _conn; }
      set
      {
        _conn = value;
        if (_conn != null)
        {
          _export = new ExportProcessor(_conn);
          _install = new InstallProcessor(_conn);
        }
      }
    }
    public IEnumerable<ConnectionData> ConnectionInfo { get; set; }
    public ExportProcessor ExportProcessor { get { return _export; } }
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
