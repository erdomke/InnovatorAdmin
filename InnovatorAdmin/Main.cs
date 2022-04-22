using Innovator.Client;
using InnovatorAdmin.Connections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public partial class Main : FormBase, IWizard
  {
    private IAsyncConnection _conn;
    private Stack<IWizardStep> _history = new Stack<IWizardStep>();
    private ExportProcessor _export;
    private InstallProcessor _install;
    private bool _updateCheckComplete = false;

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
      get { return pnlConnectionColor.ShadowColor; }
      set
      {
        pnlConnectionColor.ShadowColor = value;
        pnlLeft.ShadowColor = value;
        pnlRight.ShadowColor = value;

        var logo = new Logo(value);
        picLogo.Image = logo.Image;
        this.Icon = logo.Icon;
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

      InitializeDpi();

      picLogo.Image = Properties.Resources.logo_black_opaque;
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
        var scale = DpiScale / (Properties.Settings.Default.LastDpiScale <= 0 ? 1 : Properties.Settings.Default.LastDpiScale);
        Properties.Settings.Default.LastDpiScale = DpiScale;
        Properties.Settings.Default.Save();
        if (Math.Round(scale, 5) != 1)
          bounds = new Rectangle(bounds.X, bounds.Y, (int)(bounds.Width * scale), (int)(bounds.Height * scale));

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
  }
}
