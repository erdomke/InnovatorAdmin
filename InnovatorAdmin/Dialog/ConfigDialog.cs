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
  public partial class ConfigDialog<T> : FormBase
  {
    private PropertyFilter<T> _filter = new PropertyFilter<T>();
    private T _dataSource;

    public PropertyFilter<T> Filter
    {
      get { return _filter; }
    }

    public string Message
    {
      get { return lblMessage.Text; }
      set { lblMessage.Text = value; }
    }

    public T DataSource
    {
      get { return _dataSource; }
      set
      {
        if (!object.Equals(_dataSource, value))
        {
          _dataSource = value;
          var items = new [] { _dataSource };
          var source = new BindingSource() { DataSource = items };
          paramGrid.SetDataSource(source);
        }
      }
    }

    public ConfigDialog()
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

    protected virtual void OnOk() { }

    private void btnOK_Click(object sender, EventArgs e)
    {
      try
      {
        if (this.ValidateChildren())
        {
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
