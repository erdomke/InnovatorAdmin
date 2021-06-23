using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public partial class ProgressDialog : Form, IProgressDialog
  {
    private Thread _background;
    private LightBox _box;
    private System.Windows.Forms.Timer _clock = new System.Windows.Forms.Timer();
    private DateTime _start = DateTime.UtcNow;
    private bool _isMarquee = true;

    public Action<IProgressDialog> Worker { get; set; }

    public ProgressDialog()
    {
      InitializeComponent();

      this.StartPosition = FormStartPosition.CenterParent;
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.ShowInTaskbar = false;

      _clock.Interval = 250;
      _clock.Tick += _clock_Tick;
      _clock.Enabled = true;
    }

    public void SetProgress(int value)
    {
      this.Invoke((Action)delegate ()
      {
        if (_isMarquee)
        {
          GlobalProgress.Instance.Start();
          progBar.Style = ProgressBarStyle.Continuous;
          _isMarquee = false;
        }
        GlobalProgress.Instance.Value(value);
        progBar.Maximum = Math.Max(value, 100);
        progBar.Value = value;
      });
    }

    void _clock_Tick(object sender, EventArgs e)
    {
      lblTime.Text = (DateTime.UtcNow - _start).ToString(@"hh\:mm\:ss");
    }

    public new DialogResult ShowDialog(IWin32Window owner)
    {
      if (this.Worker == null)
        throw new ArgumentNullException("Worker");

      var ctrl = owner as Control;
      if (ctrl != null)
      {
        _box = new LightBox();
        _box.Show(ctrl.TopLevelControl, this);
      }
      _background = new Thread(PerformAction);
      _background.Start(this);
      GlobalProgress.Instance.Marquee();
      return base.ShowDialog(owner);
    }

    private void PerformAction(object dialog)
    {
      this.Worker.Invoke((IProgressDialog)dialog);
      var st = Stopwatch.StartNew();
      while (!this.IsHandleCreated && st.Elapsed < TimeSpan.FromSeconds(3))
        Thread.Sleep(100);
      this.Invoke((Action)delegate()
      {
        this.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.Close();
      });
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
      _background.Abort();
      this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.Close();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
      base.OnFormClosing(e);
      GlobalProgress.Instance.Stop();
      if (_box != null)
        _box.Dispose();
    }

    public static DialogResult Display(IWin32Window owner, Action<IProgressDialog> worker)
    {
      using (var dialog = new ProgressDialog())
      {
        dialog.Worker = worker;
        return dialog.ShowDialog(owner);
      }
    }
  }

  public interface IProgressDialog
  {
    void SetProgress(int value);
  }
}
