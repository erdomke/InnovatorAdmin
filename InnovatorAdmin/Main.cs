using InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Innovator.Client;
using System.Reflection;
using System.Drawing;

namespace InnovatorAdmin
{
  public partial class Main : FormBase, IWizard, IUpdateListener
  {
    private IAsyncConnection _conn;
    private Stack<IWizardStep> _history = new Stack<IWizardStep>();
    private ExportProcessor _export;
    //private ImportProcessor _import;
    private InstallProcessor _install;
    private bool _updateCheckComplete = false;
    private SimpleToolstripRenderer _renderer = new SimpleToolstripRenderer();

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
          if (this.ConnectionInfo.Count() == 1)
          {
            this.ConnectionColor = this.ConnectionInfo.First().Color;
          }
        }
      }
    }
    public IEnumerable<ConnectionData> ConnectionInfo { get; set; }
    public ExportProcessor ExportProcessor { get { return _export; } }
    public InstallProcessor InstallProcessor { get { return _install; } }
    public InstallScript InstallScript { get; set; }

    private Color ConnectionColor
    {
      get { return tblHeader.BackColor; }
      set
      {
        _renderer.BaseColor = value;
        tblHeader.BackColor = value;

        picLogo.Image = _renderer.ColorTable.SeparatorDark.GetBrightness() < 0.5
          ? Properties.Resources.logo_black
          : Properties.Resources.logo_white;

        pnlLeftTop.BackColor = value;
        pnlTopLeft.BackColor = value;
        pnlTop.BackColor = value;
        pnlTopRight.BackColor = value;
        pnlRightTop.BackColor = value;

        this.ActiveTextColor = _renderer.ColorTable.SeparatorDark;
        this.DownBackColor = _renderer.ColorTable.ButtonPressedGradientBegin;
        this.DownTextColor = _renderer.ColorTable.SeparatorDark;
        this.HoverBackColor = _renderer.ColorTable.ButtonSelectedGradientBegin;
        this.HoverTextColor = _renderer.ColorTable.SeparatorDark;
      }
    }

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

      this.TitleLabel = lblTitle;
      this.MaximizeLabel = lblMaximize;
      this.MinimizeLabel = lblMinimize;
      this.CloseLabel = lblClose;
      this.LeftBorderPanel = pnlLeft;
      this.TopLeftCornerPanel = pnlTopLeft;
      this.TopLeftPanel = pnlLeftTop;
      this.TopBorderPanel = pnlTop;
      this.TopRightCornerPanel = pnlTopRight;
      this.TopRightPanel = pnlRightTop;
      this.RightBorderPanel = pnlRight;
      this.BottomRightCornerPanel = pnlBottomRight;
      this.BottomBorderPanel = pnlBottom;
      this.BottomLeftCornerPanel = pnlBottomLeft;
      this.InitializeTheme();

      _renderer = new SimpleToolstripRenderer();
      this.ConnectionColor = Color.LightGray;
      picLogo.MouseDown += SystemLabel_MouseDown;
      picLogo.MouseUp += SystemLabel_MouseUp;

      var assy = Assembly.GetExecutingAssembly().GetName().Version;
      this.lblVersion.Text = "v" + assy.ToString();
    }

    protected override void OnLoad(EventArgs e)
    {
      try
      {
        base.OnLoad(e);

        var bounds = Properties.Settings.Default.Main_Bounds;
        if (bounds.Width < 100 || bounds.Height < 100)
        {
          // Do nothing
        }
        else if (bounds.IntersectsWith(SystemInformation.VirtualScreen))
        {
          this.DesktopBounds = bounds;
        }
        else
        {
          this.Size = bounds.Size;
        }

        if (!_history.Any()) GoToStep(new Controls.Welcome());
        //GoToStep(new Controls.MergeInterface().Initialize(new HgMergeOperation(@"C:\Users\edomke\Documents\Local_Projects\ArasUpgrade", 2, 1)));
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }

    private DateTime _lastFileWrite = DateTime.Now;

    public void GoToStep(IWizardStep step)
    {
      if (step is Controls.Welcome) _history.Clear();
      var ctrl = step as Control;
      if (ctrl == null) throw new ArgumentException("Each step must be a control.");

      var curr = tblMain.GetControlFromPosition(1, 3);
      if (curr != null) tblMain.Controls.Remove(curr);

      ctrl.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
      tblMain.Controls.Add(ctrl, 1, 3);
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

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
      base.OnFormClosed(e);
      SaveFormBounds();
    }
    protected override void OnMove(EventArgs e)
    {
      base.OnMove(e);
      SaveFormBounds();
    }
    protected override void OnResizeEnd(EventArgs e)
    {
      base.OnResizeEnd(e);
      SaveFormBounds();
    }
    private void SaveFormBounds()
    {
      if (this.WindowState == FormWindowState.Normal)
      {
        Properties.Settings.Default.Main_Bounds = this.DesktopBounds;
        Properties.Settings.Default.Save();
        Properties.Settings.Default.Reload();
      }
    }

    public void UpdateCheckComplete(Version latestVersion)
    {
      try
      {
        _updateCheckComplete = true;
        var currVer = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        if (latestVersion == default(Version))
        {
          this.lblVersion.Text = string.Format("v{0} (No updates available)", currVer);
        }
        else
        {
          var newVer = latestVersion.ToString();

          if (newVer != currVer)
          {
            this.lblVersion.Text = string.Format("v{0} (Restart to install v{1}!)", currVer, newVer);
          }
          else
          {
            this.lblVersion.Text = string.Format("v{0} (No updates available)", currVer);
          }
        }
      }
      catch (Exception) { }
    }

    public void UpdateCheckProgress(int progress)
    {
      try
      {
        if (!_updateCheckComplete)
        {
          var currVer = Assembly.GetExecutingAssembly().GetName().Version.ToString();
          this.lblVersion.Text = string.Format("v{0} (Checking updates: {1}%)", currVer, progress);
        }
      }
      catch (Exception) { }
    }
  }
}
