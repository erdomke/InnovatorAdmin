using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InnovatorAdmin
{
  public partial class ProgressDialog : Form
  {
    private Thread _background;
    private LightBox _box;
    private System.Windows.Forms.Timer _clock = new System.Windows.Forms.Timer();
    private DateTime _start = DateTime.UtcNow;

    public Action Worker { get; set; }

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
      _background.Start();
      GlobalProgress.Instance.Marquee();
      return base.ShowDialog(owner);
    }

    private void PerformAction()
    {
      this.Worker.Invoke();
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

    public static DialogResult Display(IWin32Window owner, Action worker)
    {
      using (var dialog = new ProgressDialog())
      {
        dialog.Worker = worker;
        return dialog.ShowDialog(owner);
      }
    }
  }
}
