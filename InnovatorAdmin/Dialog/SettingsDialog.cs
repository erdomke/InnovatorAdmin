using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InnovatorAdmin.Dialog
{
  public partial class SettingsDialog : FormBase
  {
    private PropertyFilter<Settings> _filter = new PropertyFilter<Settings>();
    private Settings _settings;

    public PropertyFilter<Settings> Filter
    {
      get { return _filter; }
    }

    public string Message
    {
      get { return lblMessage.Text; }
      set { lblMessage.Text = value; }
    }

    public Settings Settings
    {
      get { return _settings; }
      set
      {
        if (_settings != value)
        {
          _settings = value;
          var items = new Settings[] { _settings };
          var source = new BindingSource() { DataSource = items };
          paramGrid.SetDataSource(source);
        }
      }
    }

    public SettingsDialog()
    {
      InitializeComponent();

      this.TitleLabel = lblTitle;
      this.TopLeftCornerPanel = pnlTopLeft;
      this.TopBorderPanel = pnlTop;
      this.TopRightCornerPanel = pnlTopRight;
      this.LeftBorderPanel = pnlLeft;
      this.RightBorderPanel = pnlRight;
      this.BottomRightCornerPanel = pnlBottomRight;
      this.BottomBorderPanel = pnlBottom;
      this.BottomLeftCornerPanel = pnlBottomLeft;
      this.InitializeTheme();

      this.KeyPreview = true;
      this.Icon = (this.Owner ?? Application.OpenForms[0]).Icon;

      paramGrid.Filter = _filter;
    }

    protected override void OnLoad(EventArgs e)
    {
      base.OnLoad(e);
      if (_settings == null)
      {
        this.Settings = Settings.Current;
      }
    }

    public DialogResult ShowDialog(IWin32Window owner, Rectangle bounds)
    {
      if (bounds != default(Rectangle))
      {
        this.StartPosition = FormStartPosition.Manual;
        var screenDim = SystemInformation.VirtualScreen;
        var newX = Math.Min(Math.Max(bounds.X, 0), screenDim.Width - this.DesktopBounds.Width);
        var newY = Math.Min(Math.Max(bounds.Y - 30, 0), screenDim.Height - this.DesktopBounds.Height);
        this.DesktopLocation = new Point(newX, newY);
      }
      return this.ShowDialog(owner);
    }

    private void btnOK_Click(object sender, EventArgs e)
    {
      try
      {
        if (this.ValidateChildren())
        {
          _settings.Save();
          this.DialogResult = System.Windows.Forms.DialogResult.OK;
          this.Close();
        }
      }
      catch (Exception ex)
      {
        Utils.HandleError(ex);
      }
    }
  }
}
